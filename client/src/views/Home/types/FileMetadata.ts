interface FileMetadata {
    filename: string;
    progress: number;
    size: number;
    done: boolean;
    uploading: boolean;
    handle: File;
    uuid: string;
}

export default FileMetadata;