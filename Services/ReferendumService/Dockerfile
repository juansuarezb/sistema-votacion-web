FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Services/ReferendumService/ReferendumService.csproj", "Services/ReferendumService/"]
RUN dotnet restore "Services/ReferendumService/ReferendumService.csproj"

COPY . .
WORKDIR "/src/Services/ReferendumService"
RUN dotnet build "ReferendumService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReferendumService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReferendumService.dll"]