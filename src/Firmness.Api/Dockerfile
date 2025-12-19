FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files first
COPY ["Firmness.Api/Firmness.Api.csproj", "Firmness.Api/"]
COPY ["Firmness.Application/Firmness.Application.csproj", "Firmness.Application/"]
COPY ["Firmness.Domain/Firmness.Domain.csproj", "Firmness.Domain/"]
COPY ["Firmness.Infrastructure/Firmness.Infrastructure.csproj", "Firmness.Infrastructure/"]

# Restore
RUN dotnet restore "Firmness.Api/Firmness.Api.csproj"

# Copy all source code
COPY . .

# Publish
WORKDIR "/src/Firmness.Api"
RUN dotnet publish "Firmness.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
# Install dependencies for Excel (EPPlus) and PDF (QuestPDF) generation
# libgdiplus: for System.Drawing (EPPlus)
# libfontconfig1, fonts-*: for SkiaSharp (QuestPDF)
RUN apt-get update && apt-get install -y \
    libgdiplus \
    libfontconfig1 \
    fonts-dejavu \
    fonts-liberation \
    fontconfig \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Firmness.Api.dll"]
