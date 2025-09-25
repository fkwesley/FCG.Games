# Stage 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory
WORKDIR /app

# Copy full solution and project folders
# This assumes you are building from the root of the repository
COPY . .

# Restore NuGet packages
RUN dotnet restore FCG.Games.sln

# Build the application in Release mode
RUN dotnet build FCG.Games.sln -c Release --no-restore

# Publish the application
RUN dotnet publish API/API.csproj -c Release -o /app/publish --no-restore

# Stage 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set working directory
WORKDIR /app

# Install the New Relic agent
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
  && echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
  && wget https://download.newrelic.com/548C16BF.gpg \
  && apt-key add 548C16BF.gpg \
  && apt-get update \
  && apt-get install -y 'newrelic-dotnet-agent' \
  && rm -rf /var/lib/apt/lists/*

# Set the environment variables for New Relic
ENV CORECLR_ENABLE_PROFILING=1 \
	CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
	CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
	CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
	NEW_RELIC_LICENSE_KEY="" \
	NEW_RELIC_APP_NAME=""

# Copy published output from build stage
COPY --from=build /app/publish .

# Set the entry point of the application
ENTRYPOINT ["dotnet", "API.dll"]
