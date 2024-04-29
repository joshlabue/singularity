using Microsoft.AspNetCore.Mvc;
using server.Helpers;
using server.Models;

namespace server.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class FileUploadController : ControllerBase
{
    [HttpPost(Name = "FileUpload")]
    public IActionResult Post(IFormFile file)
    {      
        UploadChunk chunk = new UploadChunk();
        chunk.Uuid = Request.Form["uuid"];
        chunk.Chunk = Int32.Parse(Request.Form["chunk"]);

        int totalChunks = Int32.Parse(Request.Form["total"]);

        if(chunk.Chunk == 0) {
            Directory.CreateDirectory("/tmp/singularity/" + chunk.Uuid);

            FileStatus status = StatusWriter.InitStatus();
            status.State = "uploading";
            status.Uuid = chunk.Uuid;
            status.NumChunks = totalChunks;
            StatusWriter.WriteStatus(status, chunk.Uuid);

            // write the first chunk to the file
            ChunkWriter.WriteChunk(chunk.Uuid, chunk.Chunk, file);
        }

        // append chunk to file
        ChunkWriter.WriteChunk(chunk.Uuid, chunk.Chunk, file);

        Console.WriteLine($"Received chunk {chunk.Chunk} ({chunk.Chunk+1}/{totalChunks}) {chunk.Uuid}");

        if (Request.Form.ContainsKey("end"))
        {
            Console.WriteLine("end of upload");

            FileStatus status = StatusWriter.LoadStatus(chunk.Uuid);
            status.State = "uploaded";
            status.Filename = Request.Form["end"];
            StatusWriter.WriteStatus(status, chunk.Uuid);

            Task<Task> combineAndEncodeTask = new(async () => {
                await ChunkWriter.CombineChunks(chunk.Uuid);
                Transcoder.LoadFrameCount(chunk.Uuid);
                await Transcoder.Encode(chunk.Uuid);
            });

            combineAndEncodeTask.Start();
            combineAndEncodeTask.Wait();
        }

        return Ok();
    }
}