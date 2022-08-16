import BackendStatus from "./BackendStatus";

interface FileMetadata {
    filename: string;
    progress: number;
    size: number;
    done: boolean;
    uploading: boolean;
    handle: File;
    uuid: string;
    backendStatus?: BackendStatus;
}

export default FileMetadata;