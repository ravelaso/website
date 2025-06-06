# Node build stage
FROM node:latest AS node-build
WORKDIR /src

# Copy package.json and install dependencies
COPY package.json package-lock.json ./
RUN npm install

# Copy the rest of the application files
COPY . .

# Build Tailwind CSS
RUN npx tailwindcss -i ./Styles/input.css -o ./wwwroot/tailwind.css --minify

# .NET build stage
FROM mcr.microsoft.com/dotnet/sdk:latest AS dotnet-build
WORKDIR /src

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything from the Node build stage
COPY --from=node-build /src .

# Publish the .NET app
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:latest AS runtime
WORKDIR /app

# Copy the published app
COPY --from=dotnet-build /app/publish .

# Expose port
EXPOSE 8080

ENTRYPOINT ["dotnet", "website.dll"]