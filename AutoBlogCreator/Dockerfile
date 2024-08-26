#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AutoBlogCreator/AutoBlogCreator.csproj", "AutoBlogCreator/"]

RUN dotnet restore "./AutoBlogCreator/AutoBlogCreator.csproj"
COPY . .
WORKDIR "/src/AutoBlogCreator"
RUN dotnet build "./AutoBlogCreator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AutoBlogCreator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN apt-get update && \
    apt-get install -y git

RUN git clone https://github.com/proxna/newsletter-gaming.git

ENTRYPOINT ["dotnet", "AutoBlogCreator.dll"]