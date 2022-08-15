import React from 'react';
import FileMetadata from '../types/FileMetadata';
import sizeToText from '../utils/sizeToText';

interface FileListEntryProps {
    file: FileMetadata;
}

const FileListEntry = (props: FileListEntryProps) => {
    return <div key={props.file.name}>
        <span>{props.file.name}</span>
        <br />
        <span className='uploadprogress'>upload progress 0 MB / {sizeToText(props.file.size)}</span>
        
        <br/>
    </div>
}

export default FileListEntry;