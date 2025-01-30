# Phase de base : téléchargement de l'image de base
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 7082
# Phase de préparation (SDK) - Utiliser l'image SDK pour la publication
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
# Copier les sources de l'application
COPY . .
# Restaurer les dépendances
RUN dotnet restore
# Publier l'application en mode Release et mettre les artefacts dans /app
RUN dotnet publish TasksManagement_API.sln --configuration Release --output /app
# Phase finale (runtime)
FROM base AS final
WORKDIR /app
# Copier les fichiers publiés depuis la phase de build
COPY --from=build /app .
# Copier les fichiers de configuration nécessaires
COPY TasksManagement_API/appsettings.* .
# Variables d'environnement
ENV ASPNETCORE_URLS=https://+:7082
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/etc/ssl/certs/localhost.pfx
# Mot de passe du certificat SSL a faire autrement
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=lambo
# Point d'entrée de l'application
ENTRYPOINT ["dotnet", "/app/TasksManagement_API.dll"]