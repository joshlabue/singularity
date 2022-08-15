import React from 'react';
import FilePicker from './FilePicker';
import FileList from './FileList';
import useFileUpload from '../handlers/useFileUpload';

const FileManager = () => {    
    const [uploadState, onChange] = useFileUpload();

    return <div className="filemanager">
        <FilePicker pending={uploadState.pendingFiles} fileAdd={(newFiles: any) => {
            onChange(newFiles);
        }}/>
        <FileList pending={uploadState.pendingFiles} uploaded={uploadState.doneFiles} />
    </div>
}

export default FileManager;