import FileMetadata from "../types/FileMetadata";

const readFileAsync = async (filename: any) => {
    return new Promise<FileMetadata>((resolve, reject) => {
        let reader = new FileReader();

        reader.onloadend = (event: ProgressEvent) => {
            const result: FileMetadata = {
                name: filename.path,
                size: event.loaded
            }

            console.log("finished loading file: " + JSON.stringify(result));

            resolve(result);
        }

        reader.onerror = reject;

        reader.readAsArrayBuffer(filename);
    });
}

export default readFileAsync;