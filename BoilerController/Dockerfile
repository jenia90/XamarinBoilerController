FROM microsoft/dotnet:2.0.0-runtime-stretch-arm32v7 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS http://+:80
RUN apt-get update; apt-get install -yq gnupg
RUN curl -sL https://deb.nodesource.com/setup_9.x | bash -
RUN apt-get update; apt-get install -yq nodejs build-essential curl libunwind8 gettext
RUN apt-get -yq clean; rm -rf /var/lib/apt/lists/*

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY BoilerController.Api/BoilerController.Api.csproj BoilerController.Api/
WORKDIR /src/BoilerController.Api
RUN dotnet restore -nowarn:msb3202,nu1503
WORKDIR /src/BoilerController.Api
COPY BoilerController.Api/ .
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app -r linux-arm

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BoilerController.Api.dll"]
