namespace server.Helpers;

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Models;

public static class Transcoder {
    public static async Task Encode(string uuid, int crf = 32) 
    {
        string path = "/tmp/singularity/" + uuid + "/combined";
        string output = "/tmp/singularity/" + uuid + "/output.mp4";
        string log = "/tmp/singularity/" + uuid + "/ffmpeg.log";
        string reportConfig = $"FFREPORT=file=\"{log}\":level=32";
        string command = $"{reportConfig} ffmpeg -i {path} -vcodec libx264 -crf 32 {output} -loglevel quiet";

        FileStatus status = StatusWriter.LoadStatus(uuid);
        status.State = "encoding";
        StatusWriter.WriteStatus(status, uuid);
        
        ProcessStartInfo psi = new("/bin/bash", $"-c \"{command}\" > /dev/null")
        {
            RedirectStandardOutput = false,
            UseShellExecute = false,
        };

        Process process = new();
        process.StartInfo = psi;
        process.Start();
        
        while(!process.HasExited)
        {
            await Task.Delay(1000);
            CheckCurrentFrame(uuid);
        }
        
        if(process.ExitCode == 0) {
            status.State = "encoded";
            status.Size = new FileInfo(output).Length;
        }
        else {
            status.State = "error";
        }

        StatusWriter.WriteStatus(status, uuid);
    }

    public static void LoadFrameCount(string uuid)
    {
        string path = "/tmp/singularity/" + uuid + "/combined";
        string command = $"ffprobe -v error -select_streams v:0 -count_packets -show_entries stream=nb_read_packets -of csv=p=0 {path}";
        ProcessStartInfo psi = new("/bin/bash", $"-c \"{command}\"")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
        };

        Process shellProcess = new();
        shellProcess.StartInfo = psi;

        shellProcess.Start();
        shellProcess.WaitForExit();
        string output = shellProcess.StandardOutput.ReadToEnd().Split(",")[0];
        
        UInt32 frameCount = UInt32.Parse(output);
        FileStatus status = StatusWriter.LoadStatus(uuid);
        status.FrameCount = frameCount;
        StatusWriter.WriteStatus(status, uuid);
    }

    /*
     * Finds the current frame that FFmpeg is on by parsing it from the log file.
     * It can be found one the last (most recent) line, in the format "frame= N"
     */
    private static void CheckCurrentFrame(string uuid) 
    {
        string log = "/tmp/singularity/" + uuid + "/ffmpeg.log";
        string lastLine = File.ReadAllLines(log).Last();
        
        string expression = "frame= +[0-9]*";
        Regex regex = new(expression);
        
        if(regex.IsMatch(lastLine)) 
        {
            Match match = regex.Match(lastLine);
            String matchString = (match.Value).Replace(" ", "");
            UInt32 frame = UInt32.Parse(match.Value.Split('=')[1]);

            FileStatus status = StatusWriter.LoadStatus(uuid);
            status.CurrentFrame = frame;
            StatusWriter.WriteStatus(status, uuid);
        }
    }
}