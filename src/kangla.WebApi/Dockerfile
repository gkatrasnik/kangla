# Restore the Angular toolchain once; the JavaScript SDK invokes it during publish.
FROM node:20-bookworm-slim AS client-dependencies
WORKDIR /src/kangla.client

COPY src/kangla.client/package.json src/kangla.client/package-lock.json ./
RUN npm ci

# Publish the ASP.NET Core API.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# The JavaScript SDK runs the Angular publish target while publishing the API.
COPY --from=client-dependencies /usr/local /usr/local
COPY Directory.Packages.props NuGet.Config ./
COPY src/kangla.WebApi/kangla.WebApi.csproj src/kangla.WebApi/
COPY src/kangla.client/kangla.client.esproj src/kangla.client/
COPY src/kangla.client/nuget.config src/kangla.client/
COPY src/kangla.Infrastructure/kangla.Infrastructure.csproj src/kangla.Infrastructure/
COPY src/kangla.Application/kangla.Application.csproj src/kangla.Application/
COPY src/kangla.Domain/kangla.Domain.csproj src/kangla.Domain/
RUN dotnet restore src/kangla.WebApi/kangla.WebApi.csproj

COPY . ./
COPY --from=client-dependencies /src/kangla.client/node_modules ./src/kangla.client/node_modules
RUN dotnet publish src/kangla.WebApi/kangla.WebApi.csproj \
    --configuration $BUILD_CONFIGURATION \
    --output /app/publish \
    /p:UseAppHost=false

# Run with an unprivileged user. Data and logs are bind-mounted by the deployment.
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
ARG UID=1001
ARG GID=1001
WORKDIR /app

RUN groupadd --gid $GID appgroup \
    && useradd --uid $UID --gid appgroup --create-home appuser \
    && mkdir /app/data /app/logs \
    && chown -R appuser:appgroup /app

COPY --from=publish --chown=appuser:appgroup /app/publish ./
COPY --from=publish --chown=appuser:appgroup /src/src/kangla.client/dist/kangla.client/browser ./wwwroot

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_FORWARDEDHEADERS_ENABLED=true \
    ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
USER appuser
ENTRYPOINT ["dotnet", "kangla.WebApi.dll"]
