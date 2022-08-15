import { fireEvent } from '@testing-library/react';
import React, {useCallback} from 'react';
import {useDropzone} from 'react-dropzone'
import FileMetadata from '../types/FileMetadata';
import readFileAsync from '../utils/readFileAsync';
import readFileMetadata from '../utils/readFileMetadata';

interface FilePickerProps {
    fileAdd: any;
    files: FileMetadata[];
}

const FilePicker = (props: FilePickerProps) => {
   
    const onDrop = useCallback((acceptedFiles: any) => {
        let newFilePaths: FileMetadata[] = [];
        for(const file of acceptedFiles) {
            newFilePaths.push(readFileMetadata(file));
        };

        console.log('writing to props...')
        props.fileAdd([...props.files, ...newFilePaths]);
    }, [props])
    const {getRootProps, getInputProps, isDragActive} = useDropzone({onDrop})

    return <div className="filepicker" {...getRootProps()}>
        <div style={{"display": 'block'}}>
        <input {...getInputProps()} />
        {
            isDragActive?
            <span className="uploader-target">now DROP</span> :
            <span className="uploader-target">put files here</span>
        }
        
        {/* {
            props.files.map(file => {
                return <div key={file}>{file}</div>
            })
        } */}
        </div>

        
    </div>
}

export default FilePicker;