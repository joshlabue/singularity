import FileMetadata from "../types/FileMetadata";
import { axios } from "lib/axios"
import { AxiosRequestConfig } from "axios";
import config from "../config.json";

const uploadFileChunks = {
    uploadFile(file: FileMetadata, progressCallback: any, timeout = 500) {
        return new Promise(async (resolve: any) => {

            let axiosConfig: AxiosRequestConfig = {
                headers: {
                    'Content-Type': 'multipart/form-data'
                },
            }

            let chunkOffset = 0;
            let currentChunk = 0;

            let currentBlob: Blob;
            let formData = new FormData();

            formData.append('uuid', file.uuid);
            
           
            const totalChunks = Math.ceil(file.size / config.chunkSize)

            formData.append('total', totalChunks.toString());

            for(currentChunk = 0; currentChunk < totalChunks; currentChunk++) {
                const startAddress = currentChunk * config.chunkSize;
                const endAddress = startAddress + config.chunkSize;
                currentBlob = file.handle.slice(startAddress, endAddress);
                formData.set('chunk', currentChunk.toString());
                formData.set('file', currentBlob);

                if(currentChunk+1 === totalChunks) { // if this is the last chunk
                    formData.set('end', file.filename)
                }

                
                await axios.post('/api/FileUpload', formData, axiosConfig)
                progressCallback(currentChunk * config.chunkSize)
                // console.log(`finished chunk ${currentChunk+1} of ${totalChunks}`)
            }

            progressCallback(file.size);

            // console.log('done uploading file');

            resolve();           
        })
    }
}

export default uploadFileChunks;