FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["EV.AdminService.API/EV.AdminService.API.csproj", "EV.AdminService.API/"]
RUN dotnet restore "EV.AdminService.API/EV.AdminService.API.csproj"

COPY . .
WORKDIR "/src/EV.AdminService.API"
RUN dotnet build "EV.AdminService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EV.AdminService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EV.AdminService.API.dll"]
