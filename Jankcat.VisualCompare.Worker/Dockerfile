FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY *.sln ./
COPY Jankcat.VisualCompare.Worker/Jankcat.VisualCompare.Worker.csproj Jankcat.VisualCompare.Worker/
COPY Jankcat.VisualCompare.Lib/Jankcat.VisualCompare.Lib.csproj Jankcat.VisualCompare.Lib/
RUN dotnet restore
COPY . .
WORKDIR /src/Jankcat.VisualCompare.Worker
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Jankcat.VisualCompare.Worker.dll"]
