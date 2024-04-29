using server.Models;

namespace server.Helpers;

public class ChunkWriter
{
    public static void WriteChunk(string uuid, int chunk, IFormFile data) {
        string path = "/tmp/singularity/" + uuid + "/chunk" + chunk;

        using FileStream stream = new(path, FileMode.Create);
        data.CopyTo(stream);
        stream.Close();
    }

    public static async Task CombineChunks(string uuid) {
        string basePath = "/tmp/singularity/" + uuid;

        FileStatus status = StatusWriter.LoadStatus(uuid);
        int totalChunks = status.NumChunks;
        status.State = "combining";
        StatusWriter.WriteStatus(status, uuid);

        FileStream combinedStream = new(basePath + "/combined", FileMode.Create);

        for(int i = 0; i < totalChunks; i++) {
            string path = basePath + "/chunk" + i;
            if(!File.Exists(path)) {
                Console.WriteLine("Missing chunk " + i + " of " + totalChunks + " for " + uuid);
                status.State = "error";
                StatusWriter.WriteStatus(status, uuid);
                return;
            }

            await using FileStream chunkStream = new(path, FileMode.Open);
            await chunkStream.CopyToAsync(combinedStream);
        }

        combinedStream.Close();

        status.State = "combined";
        StatusWriter.WriteStatus(status, uuid);
    }
}