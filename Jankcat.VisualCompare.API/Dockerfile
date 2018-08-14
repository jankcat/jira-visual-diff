FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY *.sln ./
COPY CM.QA.Tools.VisualCompare.API/CM.QA.Tools.VisualCompare.API.csproj CM.QA.Tools.VisualCompare.API/
COPY CM.QA.Tools.VisualCompare.Lib/CM.QA.Tools.VisualCompare.Lib.csproj CM.QA.Tools.VisualCompare.Lib/
RUN dotnet restore
COPY . .
WORKDIR /src/CM.QA.Tools.VisualCompare.API
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CM.QA.Tools.VisualCompare.API.dll"]
