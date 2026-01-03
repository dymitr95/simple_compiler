FROM mcr.microsoft.com/dotnet/sdk:10.0 AS app

ARG APP_USER=app
USER app

# Working directories
WORKDIR /home/$APP_USER/src
RUN mkdir -p /home/$APP_USER/publish

# Copy project and restore
COPY --chown=$APP_USER:$APP_USER ["SimpleCompiler.csproj", "./"]
RUN dotnet restore

# Copy source
COPY --chown=$APP_USER:$APP_USER . .

# Publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c ${BUILD_CONFIGURATION} -o /home/$APP_USER/publish /p:UseAppHost=false

# Set working directory to published app
WORKDIR /home/$APP_USER/publish

# Expose ports
EXPOSE 8080 8081

VOLUME ["/home/$APP_USER/publish/appsettings.json"]
VOLUME ["/home/$APP_USER/publish/appsettings.Development.json"]

# Run the app
ENTRYPOINT ["dotnet", "SimpleCompiler.dll"]