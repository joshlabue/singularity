import FileMetadata from "../types/FileMetadata";
import { axios } from "lib/axios"

const downloadFile = {
    downloadFile(file: FileMetadata, timeout = 500) {
        return new Promise(async (resolve: any) => {
            axios.get('/api/Retrieval/Download/' + file.uuid, {responseType: 'blob'}).then((response) => {
                const url = window.URL.createObjectURL(new Blob([response.data]))
                const link = document.createElement('a');
                link.href = url;
                link.setAttribute('download', file.filename);
                document.body.appendChild(link);
                link.click();

                resolve();
            })            
        })
    }
}

export default downloadFile;