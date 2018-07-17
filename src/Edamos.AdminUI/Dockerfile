FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Edamos.sln ./
COPY Edamos.AdminUI/Edamos.AdminUI.csproj Edamos.AdminUI/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/Edamos.AdminUI
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY root.crt /usr/local/share/ca-certificates/EdamosRootCA.crt
ENTRYPOINT ["dotnet", "Edamos.AdminUI.dll"]
