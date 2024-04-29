using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using server.Models;


namespace server.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class RetrievalController : ControllerBase
{
    [HttpGet("Query")]
    public string Query()
    {
        string query = Request.Query["uuids"];
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
        string uuid = Request.RouteValues["uuid"]!.ToString()!;
        
        string path = "/tmp/singularity/" + uuid + "/output.mp4";

        return PhysicalFile(path, "video/mp4");
    }
}