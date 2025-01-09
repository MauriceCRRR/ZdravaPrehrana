# Osnovna slika za zagon aplikacije
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Gradnja aplikacije
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Kopiranje .csproj datoteke in obnavljanje odvisnosti
COPY ["ZdravaPrehrana.csproj", "./"]
RUN dotnet restore "ZdravaPrehrana.csproj"

# Kopiranje preostale kode in gradnja
COPY . .
RUN dotnet build "ZdravaPrehrana.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Objavljanje aplikacije
FROM build AS publish
RUN dotnet publish "ZdravaPrehrana.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Konèna slika za zagon
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZdravaPrehrana.dll"]
