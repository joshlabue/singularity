FROM mcr.microsoft.com/dotnet/sdk:6.0-jammy

RUN apt-get update && apt-get -y upgrade
RUN apt-get install -y ffmpeg 

COPY server /build
WORKDIR /build
RUN dotnet publish -o /singularity

EXPOSE 5000
ENTRYPOINT ["dotnet", "/singularity/server.dll", "--urls", "http://*:5000"]
