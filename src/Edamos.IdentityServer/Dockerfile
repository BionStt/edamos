FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY Edamos.IdentityServer/Edamos.IdentityServer.csproj Edamos.IdentityServer/
RUN dotnet restore Edamos.IdentityServer/Edamos.IdentityServer.csproj
COPY . .
WORKDIR /src/Edamos.IdentityServer
RUN dotnet build Edamos.IdentityServer.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Edamos.IdentityServer.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY root.crt /usr/local/share/ca-certificates/foo.crt
RUN update-ca-certificates
ENTRYPOINT ["dotnet", "Edamos.IdentityServer.dll"]
