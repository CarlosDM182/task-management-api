#Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY TaskManagement.slnx ./
COPY TaskManagement.Api/TaskManagement.Api.csproj TaskManagement.Api/
COPY TaskManagement.Application/TaskManagement.Application.csproj TaskManagement.Application/
COPY TaskManagement.Domain/TaskManagement.Domain.csproj TaskManagement.Domain/
COPY TaskManagement.Infrastructure/TaskManagement.Infrastructure.csproj TaskManagement.Infrastructure/
COPY TaskManagement.Tests/TaskManagement.Tests.csproj TaskManagement.Tests/

RUN dotnet restore

COPY . .

RUN dotnet publish TaskManagement.Api/TaskManagement.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

#Runtime stage

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "TaskManagement.Api.dll"]