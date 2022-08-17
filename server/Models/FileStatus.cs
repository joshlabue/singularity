class FileStatus 
{
    public string state { get; set; }
    public string uuid { get; set; }
    public string filename { get; set; }
    public int size { get; set; }
    public int numChunks { get; set; }
    public uint frameCount { get; set; }
    public uint currentFrame { get; set; }

}