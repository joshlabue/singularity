import FileMetadata from "../types/FileMetadata";
import { axios } from "lib/axios"
import { AxiosRequestConfig } from "axios";

const queryFiles = {
    queryFiles(files: FileMetadata[], timeout = 500) {
        return new Promise(async (resolve: any) => {

            let uuids: string = '';

            for(let i = 0; i < files.length; i++) {
                if(i != 0) uuids += ','
                uuids += files[i].uuid;
            }
     
            let config: AxiosRequestConfig = {
                params: {
                    uuids:uuids
                }
            }

            
            axios.get('/api/Retrieval/Query', config).then((response) => {
                resolve(response.data);
            })            
        })
    }
}

export default queryFiles;