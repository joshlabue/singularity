import React from 'react';
import FileMetadata from '../types/FileMetadata';
import FileListEntry from './FileListEntry';

interface FileListProps {
    pending: FileMetadata[];
    uploaded: FileMetadata[];

}

const FileList = (props: FileListProps) => {
    return <div className='filelist'>
        <div>
            {
                props.uploaded.map((file: FileMetadata) => {
                    return <FileListEntry key={file.filename} file={file} uploaded={true}/>
                })
            }
            {
                props.pending.map((file: FileMetadata) => {
                    return <FileListEntry key={file.filename} file={file} uploaded={false}/>
                })
            }
        </div>
    </div>
}

export default FileList;