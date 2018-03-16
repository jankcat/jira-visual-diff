FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY *.sln ./
COPY CM.QA.Tools.VisualCompare.Worker/CM.QA.Tools.VisualCompare.Worker.csproj CM.QA.Tools.VisualCompare.Worker/
COPY CM.QA.Tools.VisualCompare.Lib/CM.QA.Tools.VisualCompare.Lib.csproj CM.QA.Tools.VisualCompare.Lib/
RUN dotnet restore
COPY . .
WORKDIR /src/CM.QA.Tools.VisualCompare.Worker
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CM.QA.Tools.VisualCompare.Worker.dll"]
