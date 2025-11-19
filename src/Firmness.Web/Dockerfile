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

# Copiar 
COPY . .

# Restaurar el proyecto desde su ruta original dentro del repo
RUN dotnet restore "Firmness.Admin.Web/Firmness.Admin.Web.csproj"

# Compilar el proyecto usando la ruta relativa original
RUN dotnet build "Firmness.Admin.Web/Firmness.Admin.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicar para producción
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
RUN dotnet publish "Firmness.Admin.Web/Firmness.Admin.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final para ejecución
FROM base AS final
WORKDIR /app

# Copiar archivos publicados
COPY --from=publish /app/publish .

# Variables de entorno para migraciones (puedes sobreescribir desde docker-compose)
ENV ASPNETCORE_ENVIRONMENT=Production

# Comando por defecto (puede ser sobrescrito en docker-compose)
ENTRYPOINT ["dotnet", "Firmness.Admin.Web.dll"]
