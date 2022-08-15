using System.Text.Json;

class StatusWriter
{
    public static FileStatus InitStatus()
    {
        FileStatus status = new FileStatus();
        status.state = "uninitialized";
        status.filename = "";
        return status;
    }

    public static void WriteStatus(FileStatus status, string uuid)
    {
        string path = "/tmp/singularity/" + uuid + "/status.json";
        string json = JsonSerializer.Serialize(status);
        System.IO.File.WriteAllText(path, json);
    }

    public static FileStatus LoadStatus(string uuid)
    {
        string path = "/tmp/singularity/" + uuid + "/status.json";
        string json = System.IO.File.ReadAllText(path);

        FileStatus status;

        if(json == "" || json == null)
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