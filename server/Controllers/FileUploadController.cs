using Microsoft.AspNetCore.Mvc;

namespace server.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class FileUploadController : ControllerBase
{
    [HttpGet(Name = "FileUpload")]
    public string Get()
    {
        Console.WriteLine("GET on /FileUpload");
        var t = new Task<Task>(async () => {
            await Task.Delay(1000);
            Console.WriteLine("Task run");
        });

        t.Start();
        t.Wait();
        return "ok";
    }


    // allow multipart form data
    

    [HttpPost(Name = "FileUpload")]

    public IActionResult Post(IFormFile file)
    {      
        UploadChunk chunk = new UploadChunk();
        chunk.uuid = this.Request.Form["uuid"];
        chunk.chunk = Int32.Parse(this.Request.Form["chunk"]);

        int totalChunks = Int32.Parse(this.Request.Form["total"]);

        if(chunk.chunk == 0) {
            Directory.CreateDirectory("/tmp/singularity/" + chunk.uuid);

            FileStatus status = StatusWriter.InitStatus();
            status.state = "uploading";
            status.uuid = chunk.uuid;
            status.numChunks = totalChunks;
            StatusWriter.WriteStatus(status, chunk.uuid);

            // write the first chunk to the file
            ChunkWriter.WriteChunk(chunk.uuid, chunk.chunk, file);
        }

        // append chunk to file
        ChunkWriter.WriteChunk(chunk.uuid, chunk.chunk, file);

        Console.WriteLine($"Received chunk {chunk.chunk} ({chunk.chunk+1}/{totalChunks}) {chunk.uuid}");

        if (this.Request.Form.ContainsKey("end"))
        {
            Console.WriteLine("end of upload");

            FileStatus status = StatusWriter.LoadStatus(chunk.uuid);
            status.state = "uploaded";
            status.filename = this.Request.Form["end"];
            StatusWriter.WriteStatus(status, chunk.uuid);

            // start encoding

            var combineAndEncodeTask = new Task<Task>(async () => {
                // Console.WriteLine("Starting reassembly on " + chunk.uuid);
                await ChunkWriter.CombineChunks(chunk.uuid);
                // Console.WriteLine("Finished reassembly on " + chunk.uuid);
                // Console.WriteLine("Getting frame count for " + chunk.uuid);
                await FFmpeg.LoadFrameCount(chunk.uuid);
                // Console.WriteLine("Finished getting frame count for " + chunk.uuid);
                // Console.WriteLine("Starting encoding for " + chunk.uuid);
                await FFmpeg.Encode(chunk.uuid);
                // Console.WriteLine("Encoding finished for " + chunk.uuid);
            });

            combineAndEncodeTask.Start();
            combineAndEncodeTask.Wait();
        }

        return Ok();
    }
}