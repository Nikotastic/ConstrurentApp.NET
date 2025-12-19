FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy csproj files first
COPY ["Firmness.Web/Firmness.Web.csproj", "Firmness.Web/"]
COPY ["Firmness.Application/Firmness.Application.csproj", "Firmness.Application/"]
COPY ["Firmness.Domain/Firmness.Domain.csproj", "Firmness.Domain/"]
COPY ["Firmness.Infrastructure/Firmness.Infrastructure.csproj", "Firmness.Infrastructure/"]

# Restore
RUN dotnet restore "Firmness.Web/Firmness.Web.csproj"

# Copy all source code
COPY . .

# Publish
WORKDIR "/src/Firmness.Web"
RUN dotnet publish "Firmness.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
# Install dependencies for Excel generation (System.Drawing.Common support)
RUN apk add --no-cache icu-libs libgdiplus krb5-libs libssl3 libintl libuuid pango cairo gobject-introspection fontconfig ttf-dejavu
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Firmness.Web.dll"]
