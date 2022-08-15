import React, { useState } from 'react';
import FilePicker from './FilePicker';
import FileList from './FileList';
import FileMetadata from '../types/FileMetadata';

const FileManager = () => {

    const [files, setFiles] = useState<FileMetadata[]>([]);
    console.log('render filemanager. state: ' + JSON.stringify(files));

    return <div className="filemanager">
        <FilePicker files={files} fileAdd={(newFiles: any) => {
            setFiles(newFiles);
        }}/>
        <FileList files={files} />
    </div>
}

export default FileManager;