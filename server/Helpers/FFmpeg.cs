using System;
using System.Diagnostics;

class FFmpeg {
    public static async Task Encode(string uuid, int crf = 32) 
    {
        string path = "/tmp/singularity/" + uuid + "/combined";
        string output = "/tmp/singularity/" + uuid + "/output.mp4";
        string log = "/tmp/singularity/" + uuid + "/ffmpeg.log";
        string reportConfig = $"FFREPORT=file=\"{log}\":level=32";
        string command = $"{reportConfig} ffmpeg -i {path} -vcodec libx264 -crf 32 {output} -loglevel quiet";

        FileStatus status = StatusWriter.LoadStatus(uuid);
        status.state = "encoding";
        StatusWriter.WriteStatus(status, uuid);
        
        var psi = new ProcessStartInfo("/bin/bash", $"-c \"{command}\" > /dev/null")
        {
            RedirectStandardOutput = false,
            UseShellExecute = false,
        };

        var process = new Process();
        process.StartInfo = psi;

        process.Start();

        // while process is running

        while(!process.HasExited)
        {
            await Task.Delay(1000);
            CheckCurrentFrame(uuid);
        }

        // process.WaitForExit();

        if(process.ExitCode == 0) {
            status.state = "encoded";
            status.size = (int) new System.IO.FileInfo(output).Length;
        }
        else {
            status.state = "error";
        }

        StatusWriter.WriteStatus(status, uuid);
    }

    public static async Task LoadFrameCount(string uuid) {
        string path = "/tmp/singularity/" + uuid + "/combined";
        string command = $"ffprobe -v error -select_streams v:0 -count_packets -show_entries stream=nb_read_packets -of csv=p=0 {path}";
        var psi = new ProcessStartInfo("/bin/bash", $"-c \"{command}\"")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
        };

        var process = new Process();
        process.StartInfo = psi;

        process.Start();
        process.WaitForExit();
        var output = process.StandardOutput.ReadToEnd();
        var frameCount = UInt32.Parse(output);
        FileStatus status = StatusWriter.LoadStatus(uuid);
        status.frameCount = frameCount;
        StatusWriter.WriteStatus(status, uuid);
    }

    public static void CheckCurrentFrame(string uuid) {
        // Console.WriteLine("checking current frame for " + uuid);

        // open ffmpeg.log and find the last line that contains "frame=", then parse the number after "="
        string log = "/tmp/singularity/" + uuid + "/ffmpeg.log";

        // log file stream
        var logStream = new System.IO.FileStream(log, System.IO.FileMode.Open);
        var logReader = new System.IO.StreamReader(logStream);
        var lastLine = "";
        while(!logReader.EndOfStream) {
            lastLine = logReader.ReadLine();
        }
        logReader.Close();

       
        // run a regular expression on the log file to find the final match of the expression
        string expression = "frame= +[0-9]*";
        var regex = new System.Text.RegularExpressions.Regex(expression);


        if(regex.IsMatch(lastLine)) 
        {
            // Console.WriteLine("matching regex...");
            var match = regex.Match(lastLine);

            // Console.WriteLine(lastLine);

            if(match != null) {
                var matchString = match.Value;
                matchString = matchString.Replace(" ", "");
                var frame = UInt32.Parse(match.Value.Split('=')[1]);

                FileStatus status = StatusWriter.LoadStatus(uuid);
                status.currentFrame = frame;
                StatusWriter.WriteStatus(status, uuid);

                // Console.WriteLine("current frame: " + frame + " of " + status.frameCount);
            }
        }

        

    }
}