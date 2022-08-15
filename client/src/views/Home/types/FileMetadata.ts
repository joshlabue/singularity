interface FileMetadata {
    filename: string;
    progress: number;
    size: number;
    done: boolean;
    uploading: boolean;
    handle: File;
}

export default FileMetadata;