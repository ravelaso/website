
Tailwind

``sh
npx tailwindcss -i ./Styles/input.css -o ./wwwroot/tailwind.css --watch
``

#!/bin/bash

# Create folders if they don't exist
mkdir -p ./content/images/gallery
mkdir -p ./content/images/thumb
mkdir -p ./content/data

# Run Docker Compose
docker-compose up --build