FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY *.sln ./
COPY Jankcat.VisualCompare.API/Jankcat.VisualCompare.API.csproj Jankcat.VisualCompare.API/
COPY Jankcat.VisualCompare.Lib/Jankcat.VisualCompare.Lib.csproj Jankcat.VisualCompare.Lib/
RUN dotnet restore Jankcat.VisualCompare.Lib/Jankcat.VisualCompare.Lib.csproj
RUN dotnet restore Jankcat.VisualCompare.API/Jankcat.VisualCompare.API.csproj
COPY . .
WORKDIR /src/Jankcat.VisualCompare.API
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Jankcat.VisualCompare.API.dll"]
