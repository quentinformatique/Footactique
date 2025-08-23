# Build the React app
FROM node:20-alpine AS build
WORKDIR /app

COPY frontend/package*.json ./
RUN npm ci

COPY frontend/ ./

# Configure API base URL at build time (default to /api for same-origin proxying)
ARG REACT_APP_API_URL=/api
ENV REACT_APP_API_URL=${REACT_APP_API_URL}

RUN npm run build

# Serve with Caddy (TLS + reverse proxy for /api)
FROM caddy:2.8.4-alpine

# Bring in Caddyfile and static files
COPY deploy/Caddyfile /etc/caddy/Caddyfile
COPY --from=build /app/build /usr/share/caddy

EXPOSE 80 443


