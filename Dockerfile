# =====================
# Build stage
# =====================
FROM mcr.microsoft.com/dotnet/sdk:9.0-windowsservercore-ltsc2019 AS build
WORKDIR /src

COPY ["BaseAPI/API.csproj", "BaseAPI/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["EmailService/EmailService.csproj", "EmailService/"]
COPY ["RedisService/RedisService.csproj", "RedisService/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["RabbitMQContract/RabbitMQContract.csproj", "RabbitMQContract/"]

RUN dotnet restore "BaseAPI/API.csproj"

COPY . .
WORKDIR "/src/BaseAPI"
RUN dotnet publish "API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# =====================
# Runtime stage
# =====================
FROM mcr.microsoft.com/dotnet/aspnet:9.0-windowsservercore-ltsc2019
WORKDIR /app
EXPOSE 8081
EXPOSE 8082

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]
