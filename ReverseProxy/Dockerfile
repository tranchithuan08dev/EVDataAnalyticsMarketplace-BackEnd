# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj và restore riêng
COPY ["ReverseProxy/ReverseProxy.csproj", "ReverseProxy/"]
RUN dotnet restore "ReverseProxy/ReverseProxy.csproj"

# Copy toàn bộ source
COPY . .

WORKDIR "/src/ReverseProxy"
RUN dotnet build "ReverseProxy.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "ReverseProxy.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReverseProxy.dll"]
