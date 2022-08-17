class ChunkWriter
{
    public static void WriteChunk(string uuid, int chunk, IFormFile data) {
        string path = "/tmp/singularity/" + uuid + "/chunk" + chunk;
        
        using (var stream = new FileStream(path, FileMode.Create))
            {
                data.CopyTo(stream);

                stream.Close();
            }
    }

    public static async Task CombineChunks(string uuid) {
        string basePath = "/tmp/singularity/" + uuid;

        FileStatus status = StatusWriter.LoadStatus(uuid);
        int totalChunks = status.numChunks;
        status.state = "combining";
        StatusWriter.WriteStatus(status, uuid);

        var combinedStream = new FileStream(basePath + "/combined", FileMode.Create);

        for(int i = 0; i < totalChunks; i++) {
            string path = basePath + "/chunk" + i;
            if(!File.Exists(path)) {
                Console.WriteLine("Missing chunk " + i + " of " + totalChunks + " for " + uuid);
                status.state = "error";
                StatusWriter.WriteStatus(status, uuid);
                return;
            }

            using (var chunkStream = new FileStream(path, FileMode.Open))
            {
                await chunkStream.CopyToAsync(combinedStream);
            }
        }

        combinedStream.Close();

        status.state = "combined";
        StatusWriter.WriteStatus(status, uuid);
    }
}