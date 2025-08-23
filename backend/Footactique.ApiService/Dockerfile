# Build and publish the ASP.NET Core API
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy backend sources
COPY backend/ ./backend/

# Restore and publish
RUN dotnet restore backend/Footactique.sln \
 && dotnet publish backend/Footactique.ApiService/Footactique.Api.csproj -c Release -o /app/publish --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "Footactique.Api.dll"]


