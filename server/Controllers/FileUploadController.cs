using Microsoft.AspNetCore.Mvc;

namespace server.Controllers;

[ApiController]
[Route("[controller]")]
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

        if(chunk.chunk == 0) {
            Directory.CreateDirectory("/tmp/singularity/" + chunk.uuid);

            FileStatus status = StatusWriter.InitStatus();
            status.state = "uploading";
            StatusWriter.WriteStatus(status, chunk.uuid);

            // write the first chunk to the file
            using (var stream = new FileStream("/tmp/singularity/" + chunk.uuid + "/original", FileMode.Create))
            {
                file.CopyTo(stream);

                stream.Flush();
            }
        }

        // append chunk to file
        using (var stream = new FileStream("/tmp/singularity/" + chunk.uuid + "/original", FileMode.Append))
        {
            file.CopyTo(stream);

            stream.Flush();
        }

        Console.WriteLine("Received chunk " + chunk.chunk + " of " + chunk.uuid);

        if (this.Request.Form.ContainsKey("end"))
        {
            Console.WriteLine("end of upload");

            FileStatus status = StatusWriter.LoadStatus(chunk.uuid);
            status.state = "uploaded";
            status.filename = this.Request.Form["end"];
            StatusWriter.WriteStatus(status, chunk.uuid);

            // start encoding

            var encodeTask = new Task<Task>(async () => {
                Console.WriteLine("Starting encoding for " + chunk.uuid);
                await FFmpeg.Encode(chunk.uuid);
                Console.WriteLine("Encoding finished for " + chunk.uuid);
            });

            encodeTask.Start();
            encodeTask.Wait();
        }

        return Ok();
    }
}