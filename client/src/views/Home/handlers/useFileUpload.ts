import { useEffect, useReducer } from 'react'
import uploadFile from '../api/uploadFile';
import FileMetadata from '../types/FileMetadata';
import {v4 as uuidv4} from 'uuid';

enum UploaderStatus {
    IDLE = 'idle',
    UPLOADING = 'uploading',
    DONE = 'done'
}

interface FileUploaderState {
    pendingFiles: FileMetadata[];
    doneFiles: FileMetadata[];
    status: UploaderStatus;
}


const initialState: FileUploaderState = {
    pendingFiles: [],
    doneFiles: [],
    status: UploaderStatus.IDLE
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
            console.log('got dispatch for file uploaded')

            let isDone = false;
            if(state.pendingFiles.length === 1) {
                isDone = true;
            }

            return {
                ...state,
                status: isDone ? UploaderStatus.DONE : UploaderStatus.UPLOADING,
                pendingFiles: [...state.pendingFiles.slice(1)],
                doneFiles: [...state.doneFiles, {...state.pendingFiles[0], done: true, uploading: false}]
            };
        default:
            return state;
    }
}

const useFileUpload = () => {
    const [state, dispatch] = useReducer(reducer, initialState);

    const onChange: any = (newFiles: any) => {
        console.log('onchange called: ' + JSON.stringify(newFiles))
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
                uploadFile.uploadFile(state.pendingFiles[0], (progress: number) => {
                    dispatch({type: 'progress', progress: progress})
                }, 3000)
                .then((fileStatus: any) => {
                    console.log('callback: api done');
                    dispatch({type: 'uploaded'})

                });
            }
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