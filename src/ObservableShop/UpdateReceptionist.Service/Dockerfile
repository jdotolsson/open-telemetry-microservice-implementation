#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UpdateReceptionist.Service/UpdateReceptionist.Service.csproj", "UpdateReceptionist.Service/"]
RUN dotnet restore "./UpdateReceptionist.Service/./UpdateReceptionist.Service.csproj"
COPY . .
WORKDIR "/src/UpdateReceptionist.Service"
RUN dotnet build "./UpdateReceptionist.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UpdateReceptionist.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UpdateReceptionist.Service.dll"]