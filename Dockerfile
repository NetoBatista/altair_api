FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /app

COPY . ./

WORKDIR /app/Altair.Api

RUN dotnet restore

RUN dotnet publish -c Release -o out

ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "./out/Altair.Api.dll"]