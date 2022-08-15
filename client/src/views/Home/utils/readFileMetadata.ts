import FileMetadata from "../types/FileMetadata";

const readFileMetadata = (file: any) => {
    const result: FileMetadata = {
        name: file.path,
        size: file.size
    }
    
    return result;
}

export default readFileMetadata;