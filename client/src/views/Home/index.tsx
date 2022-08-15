import React from 'react';
import FileManager from './components/FileManager';
import config from './config.json';
import sizeToText from './utils/sizeToText';


class Home extends React.Component {
    render() {
        return <div className="container-main">
            <div className="header">singularity</div>
            <div className="subheader">video compressor</div>

            <div className="controls">
                <FileManager />
                {
                    config["devMode"] ?
                    <div className='dev'>
                        developer
                    </div>:
                    <div />
                }
            </div>
        </div>
    }
}

export default Home;