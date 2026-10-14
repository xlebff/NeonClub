# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости (кэшируемый слой)
COPY ["NeonClub.Api/NeonClub.Api.csproj", "NeonClub.Api/"]
COPY ["NeonClub.Shared/NeonClub.Shared.csproj", "NeonClub.Shared/"]
RUN dotnet restore "NeonClub.Api/NeonClub.Api.csproj"

# Копируем весь код и публикуем
COPY . .
RUN dotnet publish "NeonClub.Api/NeonClub.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# .NET 8+ образы уже используют non-root пользователя app
USER app

COPY --from=build /app/publish .

# Порт, который слушает Kestrel внутри контейнера
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "NeonClub.Api.dll"]