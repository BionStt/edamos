FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
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
ENTRYPOINT ["dotnet", "Edamos.IdentityServer.dll"]
