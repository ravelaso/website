# Personal Website

Its made with Blazor (.NET) uses OAuth with github to facilitate a backend to publish content like a wordpress.

(Work in progress)


Tailwind

```sh
npx tailwindcss -i ./Styles/input.css -o ./wwwroot/tailwind.css --watch
```


prepare-folders.sh

```sh
#!/bin/bash

# Create folders if they don't exist
mkdir -p ./content/images/gallery
mkdir -p ./content/images/thumb
mkdir -p ./content/data

# Run Docker Compose
docker-compose up -d
```
