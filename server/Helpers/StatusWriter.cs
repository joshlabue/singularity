using System.Text.Json;
using server.Models;

class StatusWriter
{
    public static FileStatus InitStatus()
    {
        FileStatus status = new FileStatus();
        status.State = "uninitialized";
        status.Uuid = "";
        status.Filename = "";
        status.Size = -1;
        return status;
    }

    public static void WriteStatus(FileStatus status, string uuid)
    {
        string path = "/tmp/singularity/" + uuid + "/status.json";
        string json = JsonSerializer.Serialize(status);
        File.WriteAllText(path, json);
    }

    public static FileStatus LoadStatus(string uuid)
    {
        string path = "/tmp/singularity/" + uuid + "/status.json";
        string json = File.ReadAllText(path);

        FileStatus status;

        if(String.IsNullOrEmpty(json))
        {
            status = InitStatus();
        }
        else
        {
            status = JsonSerializer.Deserialize<FileStatus>(json)!;
        }
        
        return status;
    }
}