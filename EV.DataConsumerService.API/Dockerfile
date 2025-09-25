FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["EV.DataConsumerService.API/EV.DataConsumerService.API.csproj", "EV.DataConsumerService.API/"]
RUN dotnet restore "EV.DataConsumerService.API/EV.DataConsumerService.API.csproj"

COPY . .
WORKDIR "/src/EV.DataConsumerService.API"
RUN dotnet build "EV.DataConsumerService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EV.DataConsumerService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EV.DataConsumerService.API.dll"]
