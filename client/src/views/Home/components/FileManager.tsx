import React, { useState } from 'react';
import FilePicker from './FilePicker';
import FileList from './FileList';
import FileMetadata from '../types/FileMetadata';
import useFileUpload from '../handlers/useFileUpload';

const FileManager = () => {

    const [files, setFiles] = useState<FileMetadata[]>([]);
    // console.log('render filemanager. state: ' + JSON.stringify(files));
    
    const [uploadState, onChange] = useFileUpload();

    return <div className="filemanager">
        <FilePicker pending={uploadState.pendingFiles} fileAdd={(newFiles: any) => {
            onChange(newFiles);
        }}/>
        <FileList pending={uploadState.pendingFiles} uploaded={uploadState.doneFiles} />
    </div>
}

export default FileManager;