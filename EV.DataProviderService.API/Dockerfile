FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["EV.DataProviderService.API/EV.DataProviderService.API.csproj", "EV.DataProviderService.API/"]
RUN dotnet restore "EV.DataProviderService.API/EV.DataProviderService.API.csproj"

COPY . .
WORKDIR "/src/EV.DataProviderService.API"
RUN dotnet build "EV.DataProviderService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EV.DataProviderService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EV.DataProviderService.API.dll"]
