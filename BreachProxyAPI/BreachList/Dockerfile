# 1) Build stage: restore, build & publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj(s) and restore first (leverages layer cache)
COPY *.csproj ./
RUN dotnet restore

# copy the rest of your API code
COPY . ./

# publish to /app/publish (Release config)
RUN dotnet publish -c Release -o /app/publish

# 2) Runtime stage: only the runtime bits
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# copy published files from build stage
COPY --from=build /app/publish .

# expose port 80 (Render will pick it up)
EXPOSE 80

# command to run your API
ENTRYPOINT ["dotnet", "BreachList.dll"]
