FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
ENV TZ=America/Sao_Paulo
WORKDIR /app
EXPOSE 80
EXPOSE 445

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build

WORKDIR /src
COPY ["src/Notification.Api/Notification.Api.csproj", "src/Notification.Api/"]
COPY . .
WORKDIR "src/Notification.Api"
RUN dotnet build "Notification.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Notification.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY .devops/wait-for-it.sh /wait-for-it.sh

RUN chmod +x /wait-for-it.sh

ENTRYPOINT ["/bin/bash", "-c", "/wait-for-it.sh db:5432 --timeout=10 -- echo 'Postgres ready.' && dotnet Notification.Api.dll"]
