import React from 'react';
import FileManager from './components/FileManager';


class Home extends React.Component {
    render() {
        return <div className="container-main">
            <div className="header">singularity</div>
            <div className="subheader">video compressor</div>

            <div className="controls">
                <FileManager />
            </div>
        </div>
    }
}

export default Home;