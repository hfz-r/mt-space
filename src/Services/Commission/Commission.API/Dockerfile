FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build
WORKDIR /src

COPY ["MT-Space.sln", ""]
COPY ["BuildingBlocks/Caching/RedisCache/RedisCache.csproj", "BuildingBlocks/Caching/RedisCache/"]
COPY ["BuildingBlocks/EventBus/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/EventBus/"]
COPY ["BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "BuildingBlocks/EventBus/EventBusRabbitMQ/"]
COPY ["BuildingBlocks/EventBus/IntegrationEventLog/IntegrationEventLog.csproj", "BuildingBlocks/EventBus/IntegrationEventLog/"]
COPY ["BuildingBlocks/Host/WebHost.Extensions/WebHost.Extensions.csproj", "BuildingBlocks/Host/WebHost.Extensions/"]
COPY ["Services/Commission/Commission.API/Commission.API.csproj", "Services/Commission/Commission.API/"]
COPY ["Services/Commission/Commission.Domain/Commission.Domain.csproj", "Services/Commission/Commission.Domain/"]
COPY ["Services/Commission/Commission.Infrastructure/Commission.Infrastructure.csproj", "Services/Commission/Commission.Infrastructure/"]
COPY ["Services/Commission/Commission.FunctionalTests/Commission.FunctionalTests.csproj", "Services/Commission/Commission.FunctionalTests/"]
COPY ["Services/Commission/Commission.UnitTests/Commission.UnitTests.csproj", "Services/Commission/Commission.UnitTests/"]
COPY ["Client/WebStatus/WebStatus.csproj", "Client/WebStatus/"]
COPY ["docker-compose.dcproj", ""]
COPY ["NuGet.config", ""]

RUN dotnet restore "MT-Space.sln"

COPY . .
WORKDIR "/src/Services/Commission/Commission.API"
RUN dotnet publish --no-restore -c Release -o /app

FROM build as unittest
WORKDIR "/src/Services/Commission/Commission.UnitTests"

FROM build as functionaltest
WORKDIR "/src/Services/Commission/Commission.FunctionalTests"

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Commission.API.dll"]