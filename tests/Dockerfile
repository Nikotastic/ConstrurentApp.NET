FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /app

# Copy solution file
COPY Firmness.sln .

# Copy all source code
COPY . .

# Restore dependencies
RUN dotnet restore Firmness.sln

# Build the solution
RUN dotnet build Firmness.sln --no-restore -c Release

# Run tests
# The container will exit with the exit code of dotnet test
# If tests fail, exit code will be non-zero
ENTRYPOINT ["dotnet", "test", "tests/Firmness.Test/Firmness.Test.csproj", "--no-build", "-c", "Release", "--logger:trx"]
