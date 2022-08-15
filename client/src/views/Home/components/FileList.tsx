import React from 'react';
import FileMetadata from '../types/FileMetadata';
import FileListEntry from './FileListEntry';

interface FileListProps {
    files: FileMetadata[];
}

const FileList = (props: FileListProps) => {
    return <div className='filelist'>
        <div>
            {
                props.files.map((file: FileMetadata) => {
                    return <FileListEntry key={file.name} file={file}/>
                })
            }
        </div>
    </div>
}

export default FileList;