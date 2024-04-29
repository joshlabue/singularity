namespace server.Models;

public record struct FileStatus(
    string State,
    string Uuid,
    string Filename,
    long Size,
    int NumChunks,
    uint FrameCount,
    uint CurrentFrame);
