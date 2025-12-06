FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY *.sln .
COPY src/ src/
COPY tests/ tests/
RUN dotnet restore && dotnet build --no-restore -c Release && dotnet publish -c Release --no-build -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish/* .
RUN adduser -D -H app
USER app
ENTRYPOINT [ "dotnet", "API.dll" ]
