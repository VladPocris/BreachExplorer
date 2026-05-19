# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY BreachList.csproj ./
RUN dotnet restore BreachList.csproj

COPY . ./
RUN dotnet publish BreachList.csproj -c Release -o /app/publish --no-restore

# Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Render sets PORT at runtime (default 10000). ASP.NET Core 8 reads ASPNETCORE_HTTP_PORTS.
ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "BreachList.dll"]
