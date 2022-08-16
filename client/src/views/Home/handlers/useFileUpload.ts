import { useEffect, useReducer } from 'react'
import uploadFileChunks from '../api/uploadFileChunks';
import FileMetadata from '../types/FileMetadata';
import {v4 as uuidv4} from 'uuid';
import queryFiles from '../api/queryFiles';

enum UploaderStatus {
    IDLE = 'idle',
    UPLOADING = 'uploading',
    DONE = 'done'
}

interface FileUploaderState {
    pendingFiles: FileMetadata[];
    uploadedFiles: FileMetadata[];
    status: UploaderStatus;
    queryHandle: any;
}


const initialState: FileUploaderState = {
    pendingFiles: [],
    uploadedFiles: [],
    status: UploaderStatus.IDLE,
    queryHandle: 0
}

const reducer = (state: FileUploaderState, action: any) => {
    switch(action.type) {
        case 'queue':
            let queueAdditions: FileMetadata[] = [];
            for(const file of action.queue) {
                queueAdditions.push({
                    filename: file.path,
                    progress: 0,
                    size: file.size,
                    done: false,
                    uploading: false,
                    handle: file,
                    uuid: uuidv4()
                });
            }
            return {...state, pendingFiles: [...state.pendingFiles, ...queueAdditions]};
        case 'begin':
           {
            let newPending: FileMetadata[] = [...state.pendingFiles];
            newPending[0].uploading = true;
            return {...state, pendingFiles: [...newPending], status: UploaderStatus.UPLOADING}
            }
        case 'progress':
            let newPending: FileMetadata[] = [...state.pendingFiles];
            newPending[0].progress = action.progress;
            return {...state, pendingFiles: [...newPending]}
        case 'uploaded':
            // console.log('got dispatch for file uploaded')

            let isDone = false;
            if(state.pendingFiles.length === 1) {
                isDone = true;
            }

            return {
                ...state,
                status: isDone ? UploaderStatus.DONE : UploaderStatus.UPLOADING,
                pendingFiles: [...state.pendingFiles.slice(1)],
                uploadedFiles: [...state.uploadedFiles, {...state.pendingFiles[0], done: true, uploading: false}],
                //queryHandle: queryHandle
            };
        case 'queryFiles':
            console.log('queryFiles dispatch');

            let uploadedFiles: FileMetadata[] = [];

            for(const file of state.uploadedFiles) {
                let foundBackend = false;
                for(const backendResult of action.query) {
                    if(file.uuid === backendResult.uuid) {
                        uploadedFiles.push({...file, backendStatus: {status: backendResult.state, size: backendResult.size}});
                        foundBackend = true;
                    }
                }
                if(foundBackend == false) {
                    uploadedFiles.push({...file})
                }
            }

            return {
                ...state,
                uploadedFiles: [...uploadedFiles]
            }
        default:
            return state;
    }
}

const useFileUpload = () => {
    const [state, dispatch] = useReducer(reducer, initialState);

    const onChange: any = (newFiles: any) => {
        // console.log('onchange called: ' + JSON.stringify(newFiles))
        let queueItems: File[] = [];
        for(const file of newFiles) {
            queueItems.push(file);
        }
        dispatch({type: 'queue', queue: queueItems});
    }

    useEffect(() => {
        if(state.pendingFiles.length) {
            if(state.pendingFiles[0].done === false && state.pendingFiles[0].uploading === false) {
                dispatch({type: 'begin'});
                uploadFileChunks.uploadFile(state.pendingFiles[0], (progress: number) => {
                    dispatch({type: 'progress', progress: progress})
                }, 3000)
                .then((fileStatus: any) => {
                    // console.log('callback: api done');
                    dispatch({type: 'uploaded'})

                });
            }
        }

        let allFilesDone = true;
        for(const file of state.uploadedFiles) {
            if(file.backendStatus?.status != 'encoded' && file.backendStatus?.status != 'error') {
                allFilesDone = false;
            }
        }
        if(allFilesDone === false) {
            let handle = setTimeout(async () => {
                console.log('query call');

                queryFiles.queryFiles(state.uploadedFiles).then((finishedFiles) => {
                    console.log('files callback: ' + JSON.stringify(finishedFiles));
                    dispatch({type: 'queryFiles', query:finishedFiles})

                });

                dispatch({type: 'queryBegin', handle: handle});
            }, 1000)
        }
    }, [state])

    return [
        state, onChange
    ]
}

export default useFileUpload;

export type {
    FileUploaderState
}