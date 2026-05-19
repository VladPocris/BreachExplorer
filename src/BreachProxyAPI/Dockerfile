FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy csproj
COPY BreachProxyAPI.csproj ./

# Restore
RUN dotnet restore

# Copy remaining files
COPY . .

# Publish
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "BreachProxyAPI.dll"]
