import FileMetadata from "../types/FileMetadata";
import { axios } from "lib/axios"

const downloadFile = {
    downloadFile(file: FileMetadata, timeout = 500) {
        return new Promise(async (resolve: any) => {
            axios.get('/api/Retrieval/Download/' + file.uuid, {responseType: 'blob'}).then((response) => {
                const url = window.URL.createObjectURL(new Blob([response.data]))
                const link = document.createElement('a');
                link.href = url;
                // set the file extension to mp4, since FFmpeg always converts to mp4
                let filename = file.filename.split('.')[0];
                filename += '.mp4';

                link.setAttribute('download', filename);
                document.body.appendChild(link);
                link.click();

                resolve();
            })            
        })
    }
}

export default downloadFile;
