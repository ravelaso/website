# Build stage
FROM mcr.microsoft.com/dotnet/sdk:latest AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Copy .env file to the publish directory if it exists
# This assumes the .env file will be present in the root of the context
#COPY .env /app/publish/

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:latest AS runtime
WORKDIR /app

# Copy the published app (including the .env file)
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

ENTRYPOINT ["dotnet", "website.dll"]
