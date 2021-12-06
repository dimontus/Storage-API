#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Api.Storage/Api.Storage.csproj", "Api.Storage/"]
ARG NUGET_URL
RUN dotnet nuget add source $NUGET_URL -n stc_nuget
RUN dotnet restore "Api.Storage/Api.Storage.csproj"
COPY . .
WORKDIR "/src/Api.Storage"
RUN dotnet build "Api.Storage.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api.Storage.csproj" -c Release -o /app/publish

FROM base AS final
COPY Certificates/*.crt /usr/local/share/ca-certificates/
RUN update-ca-certificates
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.Storage.dll"]
