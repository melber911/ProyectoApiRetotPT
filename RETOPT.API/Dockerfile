# Consulte https://aka.ms/customizecontainer para aprender a personalizar su contenedor de depuración y cómo Visual Studio usa este Dockerfile para compilar sus imágenes para una depuración más rápida.

# Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

#ENV DB_CONNECTION_STRING="Server=192.168.0.111,1433;Database=RetoPTDB2;User Id=sa;Password=123;"
# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RETOPT.API/RETOPT.API.csproj", "RETOPT.API/"]
COPY ["RETOPT.Application/RETOPT.Application.csproj", "RETOPT.Application/"]
COPY ["RETOPT.Domain/RETOPT.Domain.csproj", "RETOPT.Domain/"]
COPY ["RETOPT.Infrastructure/RETOPT.Infrastructure.csproj", "RETOPT.Infrastructure/"]
RUN dotnet restore "./RETOPT.API/RETOPT.API.csproj"
COPY . .
WORKDIR "/src/RETOPT.API"
RUN dotnet build "./RETOPT.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RETOPT.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RETOPT.API.dll"]