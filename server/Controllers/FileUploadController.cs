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
            Directory.CreateDirectory("/tmp/upload/" + chunk.uuid);

            // write the first chunk to the file
            using (var stream = new FileStream("/tmp/upload/" + chunk.uuid + "/original", FileMode.Create))
            {
                file.CopyTo(stream);

                stream.Flush();
            }
        }

        // append chunk to file
        using (var stream = new FileStream("/tmp/upload/" + chunk.uuid + "/original", FileMode.Append))
        {
            file.CopyTo(stream);

            stream.Flush();
        }

        Console.WriteLine("Received chunk " + chunk.chunk + " of " + chunk.uuid);

        if (this.Request.Form.ContainsKey("end"))
        {
            Console.WriteLine("end of upload");
        }

        return Ok();
    }
}