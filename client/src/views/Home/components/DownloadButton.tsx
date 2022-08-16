import downloadFile from '../api/downloadFile';
import FileMetadata from '../types/FileMetadata';

interface DownloadButtonProps {
    file: FileMetadata;
}

const DownloadButton = (props: DownloadButtonProps) => {
    return <span className="download" onClick={async () => {
        await downloadFile.downloadFile(props.file);
    }}>
        download
    </span>
}

export default DownloadButton;