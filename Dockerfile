# ===== Base (runtime) =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# ===== Build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar archivo de proyecto
COPY FellerBackend/FellerBackend.csproj FellerBackend/
RUN dotnet restore FellerBackend/FellerBackend.csproj

# Copiar código y compilar
COPY . .
WORKDIR /src/FellerBackend
RUN dotnet publish FellerBackend.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ===== Final =====
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# ⚠️ Puerto dinámico de Railway
ENTRYPOINT ["sh","-lc","URLS=http://0.0.0.0:${PORT:-8080} ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080} exec dotnet FellerBackend.dll"]
