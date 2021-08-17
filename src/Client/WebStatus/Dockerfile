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
COPY ["Services/Investor/Investor.API/Investor.API.csproj", "Services/Investor/Investor.API/"]
COPY ["Services/Investor/Investor.Domain/Investor.Domain.csproj", "Services/Investor/Investor.Domain/"]
COPY ["Services/Investor/Investor.Infrastructure/Investor.Infrastructure.csproj", "Services/Investor/Investor.Infrastructure/"]
COPY ["Services/Investor/Investor.FunctionalTests/Investor.FunctionalTests.csproj", "Services/Investor/Investor.FunctionalTests/"]
COPY ["Services/Investor/Investor.UnitTests/Investor.UnitTests.csproj", "Services/Investor/Investor.UnitTests/"]
COPY ["Client/WebStatus/WebStatus.csproj", "Client/WebStatus/"]
COPY ["docker-compose.dcproj", ""]
COPY ["NuGet.config", ""]

RUN dotnet restore "MT-Space.sln"

COPY . .
WORKDIR "/src/Client/WebStatus"
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebStatus.dll"]