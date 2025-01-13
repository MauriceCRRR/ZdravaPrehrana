# Osnovna slika za zagon aplikacije
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Gradnja aplikacije
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Kopiranje .csproj in libman.json
COPY ["ZdravaPrehrana.csproj", "libman.json", "./"]
RUN dotnet restore "ZdravaPrehrana.csproj"

# Kopiranje preostale kode
COPY . .

# Namesti in izvedi libman
RUN dotnet tool install -g Microsoft.Web.LibraryManager.CLI
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN libman restore

# Gradnja
RUN dotnet build "ZdravaPrehrana.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Objavljanje
FROM build AS publish
RUN dotnet publish "ZdravaPrehrana.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Konèna slika
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZdravaPrehrana.dll"]