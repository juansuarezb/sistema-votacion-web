FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Services/VoteService/VoteService.csproj", "Services/VoteService/"]
RUN dotnet restore "Services/VoteService/VoteService.csproj"

COPY . .
WORKDIR "/src/Services/VoteService"
RUN dotnet build "VoteService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VoteService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VoteService.dll"]