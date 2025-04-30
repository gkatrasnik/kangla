# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER $APP_UID  set by visual studio 
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This is needed so that the container can be run as a non-root user and still have access to the mounted volumes
# Accept UID and GID as build arguments
ARG UID=1001
ARG GID=1001

# Create non-root user and group
RUN groupadd -g $GID appgroup && useradd -m -u $UID -g appgroup appuser

# Use the non-root user
USER appuser

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS with-node
RUN apt-get update
RUN apt-get install curl
RUN curl -sL https://deb.nodesource.com/setup_20.x | bash
RUN apt-get -y install nodejs
RUN npm install -g @angular/cli

# This stage is used to build the service project
FROM with-node AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["NuGet.Config", "."]
COPY ["src/kangla.client/nuget.config", "src/kangla.client/"]
COPY ["src/kangla.WebApi/kangla.WebApi.csproj", "src/kangla.WebApi/"]
COPY ["src/kangla.client/kangla.client.esproj", "src/kangla.client/"]
COPY ["src/kangla.Infrastructure/kangla.Infrastructure.csproj", "src/kangla.Infrastructure/"]
COPY ["src/kangla.Application/kangla.Application.csproj", "src/kangla.Application/"]
COPY ["src/kangla.Domain/kangla.Domain.csproj", "src/kangla.Domain/"]
RUN dotnet restore "./src/kangla.WebApi/kangla.WebApi.csproj"
COPY . .
WORKDIR "/src/src/kangla.WebApi"
RUN dotnet build "./kangla.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./kangla.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "kangla.WebApi.dll"]