FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy csproj first
COPY src/BreachProxyAPI/BreachProxyAPI.csproj src/BreachProxyAPI/

# Restore
RUN dotnet restore src/BreachProxyAPI/BreachProxyAPI.csproj

# Copy everything else
COPY . .

# Publish
RUN dotnet publish src/BreachProxyAPI/BreachProxyAPI.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "BreachProxyAPI.dll"]
