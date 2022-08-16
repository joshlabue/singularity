import config from '../config.json'
import React from 'react';
import FileMetadata from '../types/FileMetadata';
import sizeToText from '../utils/sizeToText';
import DownloadButton from './DownloadButton';

interface FileListEntryProps {
    file: FileMetadata;
    uploaded: boolean;
}

const FileListEntry = (props: FileListEntryProps) => {

    let backendRow: any = '';

    switch(props.file.backendStatus?.status) {
        case 'encoded':
            backendRow = <span><DownloadButton file={props.file}/> {sizeToText(props.file.backendStatus.size)}</span>
            break;
        case 'error':
            backendRow = <span className='danger'>error</span>
            break;
        default:
            backendRow = 'encoding...'
    }

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
        <span className='compressprogress'>
            {
               backendRow
            }
        </span>
        <br/>
       
        <span className="uuid">
            {
                config.devMode ? `uuid: ${props.file.uuid}`:''
            }
        </span>
    </div>
}

export default FileListEntry;