FROM mcr.microsoft.com/dotnet/sdk:6.0
LABEL version="1.0.0" description="Teste MQTTnet"
COPY dist /app
WORKDIR /app
EXPOSE  80/tcp
ENTRYPOINT [ "dotnet","TesteMQTT.dll" ]