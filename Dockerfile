FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY . .
RUN dotnet restore SwissTax.sln
RUN dotnet publish src/Api/Api.csproj -c Release -o /out/api --no-restore
RUN dotnet publish src/Worker/Worker.csproj -c Release -o /out/worker --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app
COPY --from=build /out/api ./api
COPY --from=build /out/worker ./worker
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000
ENTRYPOINT ["dotnet", "api/Api.dll"]
