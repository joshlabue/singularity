import React, {useCallback} from 'react';
import {useDropzone} from 'react-dropzone'
import FileMetadata from '../types/FileMetadata';

interface FilePickerProps {
    fileAdd: any;
    pending: FileMetadata[];
}

const FilePicker = (props: FilePickerProps) => {
   
    const onDrop = useCallback((acceptedFiles: any) => {
        let newFilePaths: FileMetadata[] = [];
        for(const file of acceptedFiles) {
            newFilePaths.push(file);
        };

        console.log('current files: ' + JSON.stringify(props.pending));

        console.log('writing to props...')


        props.fileAdd([...props.pending, ...newFilePaths]);
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
        
        </div>

        
    </div>
}

export default FilePicker;