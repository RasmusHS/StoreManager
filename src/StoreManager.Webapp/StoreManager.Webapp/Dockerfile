FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files - note the paths relative to docker-compose context (root)
COPY ["src/StoreManager.Webapp/StoreManager.Webapp/StoreManager.Webapp.csproj", "src/StoreManager.Webapp/StoreManager.Webapp/"]
COPY ["src/StoreManager.Webapp/StoreManager.Webapp.Client/StoreManager.Webapp.Client.csproj", "src/StoreManager.Webapp/StoreManager.Webapp.Client/"]

# Restore dependencies
RUN dotnet restore "src/StoreManager.Webapp/StoreManager.Webapp/StoreManager.Webapp.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/src/StoreManager.Webapp/StoreManager.Webapp"
RUN dotnet build "StoreManager.Webapp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "StoreManager.Webapp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StoreManager.Webapp.dll"]