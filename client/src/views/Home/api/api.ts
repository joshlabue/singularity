import FileMetadata from "../types/FileMetadata";
import { axios } from "lib/axios"
import { AxiosRequestConfig } from "axios";

const api= {
    uploadFile(file: FileMetadata, progressCallback: any, timeout = 500) {
        return new Promise(async (resolve: any) => {

            let formData = new FormData();
            formData.append('name', 'video');
            formData.append('file', file.handle);

            let config: AxiosRequestConfig = {
                headers: {
                    'Content-Type': 'multipart/form-data'
                },
                onUploadProgress: (progressEvent: ProgressEvent) => {
                    progressCallback(progressEvent.loaded)
                }
            }

            axios.post('/', formData, config).then(() => {
                resolve();
            })            
        })
    }
}

export default api;