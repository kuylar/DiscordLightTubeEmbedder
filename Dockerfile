FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LightTubeEmbedder/LightTubeEmbedder.csproj", "LightTubeEmbedder/"]
RUN dotnet restore "LightTubeEmbedder/LightTubeEmbedder.csproj"
COPY . .
WORKDIR "/src/LightTubeEmbedder"
RUN dotnet build "LightTubeEmbedder.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LightTubeEmbedder.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LightTubeEmbedder.dll"]