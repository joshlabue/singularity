# singularity
Singularity is the overengineered solution to a problem I regularly suffer from: I have video files, and they're too big. 

Instead of installing FFmpeg like a levelheaded person, I wrote Singularity. 

Upload big video files, let it spit them back at you, compressed down.

## Installation
### Bare metal
- Install the following dependencies:
  - [.NET Core SDK](https://dotnet.microsoft.com/en-us/download)
  - FFmpeg
  - Yarn
- Clone this repo
- In the root folder, run `yarn build-prod`
- To start the server, cd into  `build` and run `server`
- The server will be up at [localhost:5000](http://localhost:5000)
### Docker
- Clone this repo
- In the root folder, run `docker build -t singularity .`
- Start the container with `docker run -p 5000:5000 -d singularity:latest`
- The app will be accessible at [localhost:5000](http://localhost:5000)

