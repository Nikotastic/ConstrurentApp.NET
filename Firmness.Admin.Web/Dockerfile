# Etapa base para ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Instalar herramientas EF Core
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Copiar proyecto principal y restaurar
COPY ["Firmness.Web/Firmness.Web.csproj", "Firmness.Web/"]
RUN dotnet restore "Firmness.Web/Firmness.Web.csproj"

# Copiar el codigo fuente
COPY . .

# Compilar el proyecto
WORKDIR "/src/Firmness.Web"
RUN dotnet build "./Firmness.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicar para producción
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Firmness.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final para ejecución
FROM base AS final
WORKDIR /app

# Copiar archivos publicados
COPY --from=publish /app/publish .

# Variables de entorno para migraciones (puedes sobreescribir desde docker-compose)
ENV ASPNETCORE_ENVIRONMENT=Production

# Comando por defecto (puede ser sobrescrito en docker-compose)
ENTRYPOINT ["dotnet", "Firmness.Web.dll"]