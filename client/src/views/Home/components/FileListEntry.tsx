import config from '../config.json'
import React from 'react';
import FileMetadata from '../types/FileMetadata';
import sizeToText from '../utils/sizeToText';

interface FileListEntryProps {
    file: FileMetadata;
    uploaded: boolean;
}

const FileListEntry = (props: FileListEntryProps) => {
    return <div key={props.file.filename}>
        <span>{props.file.filename}</span>
        <br />
        <span className='uploadprogress'>
            upload progress {sizeToText(props.file.progress)} / {sizeToText(props.file.size)}
            <span className="success">
                {
                    props.uploaded ? " ✓":""
                }
            </span>
        </span>
        <br/>
        {/* <span className='compressprogress'>compressing...</span>
        <br/> */}
       
        <span className="uuid">
            {
                config.devMode ? `uuid: ${props.file.uuid}`:''
            }
        </span>
    </div>
}

export default FileListEntry;