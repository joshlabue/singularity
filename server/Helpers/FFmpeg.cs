using System;
using System.Diagnostics;

class FFmpeg {
    public static async Task Encode(string uuid, int crf = 32) 
    {
        string path = "/tmp/singularity/" + uuid + "/original";
        string output = "/tmp/singularity/" + uuid + "/output.mp4";
        string command = $"ffmpeg -i {path} -vcodec libx264 -crf 32 {output} -loglevel quiet";

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

        await Task.Delay(1000);

        process.WaitForExit();

        if(process.ExitCode == 0) {
            status.state = "encoded";
        }
        else {
            status.state = "error";
        }

        StatusWriter.WriteStatus(status, uuid);
    }
}