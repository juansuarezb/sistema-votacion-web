FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Services/AuditService/AuditService.csproj", "Services/AuditService/"]
RUN dotnet restore "Services/AuditService/AuditService.csproj"

COPY . .
WORKDIR "/src/Services/AuditService"

RUN dotnet publish \
    "AuditService.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AuditService.dll"]