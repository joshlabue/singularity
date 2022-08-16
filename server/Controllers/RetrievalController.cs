using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace server.Controllers;

[ApiController]
[Route("[controller]")]
public class RetrievalController : ControllerBase
{
    [HttpGet]
    public string Index()
    {
        Console.WriteLine("GET on /Retrieval");

        // if path is /Retrieval/Query
        if(this.Request.Path.Value[1].Equals("Query"))
        {
            Console.WriteLine("Query");
        }

        return "ok";
    }

    // http GET on /Retrieval/Test
    [HttpGet("Query")]
    public string Query()
    {
        string query = this.Request.Query["uuids"];
        string[] uuids = query.Split(',');
        
        List<FileStatus> statuses = new List<FileStatus>();

        foreach(string uuid in uuids)
        {
            FileStatus status = StatusWriter.LoadStatus(uuid);
            statuses.Add(status);
        }

        string json = JsonSerializer.Serialize(statuses);
        return json;
    }

    [HttpGet("Download/{uuid}")]
    public FileResult Download()
    {
        string uuid = this.Request.RouteValues["uuid"]!.ToString()!;
        
        string path = "/tmp/singularity/" + uuid + "/output.mp4";

        return PhysicalFile(path, "video/mp4");
    }

}