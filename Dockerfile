# syntax=docker/dockerfile:1

# (Este ARG se puede usar en el build stage)
ARG LAB_CA_FILE=certs/labc23-root-ca.crt

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Re-declarar ARG dentro del stage para poder usarlo
ARG LAB_CA_FILE

# Instalar CA del LAB (si existe en el contexto)
COPY ${LAB_CA_FILE} /usr/local/share/ca-certificates/lab-root-ca.crt
RUN update-ca-certificates

COPY AuthService.sln ./
COPY src/AuthService.Api/AuthService.Api.csproj src/AuthService.Api/
COPY src/AuthService.Application/AuthService.Application.csproj src/AuthService.Application/
COPY src/AuthService.Domain/AuthService.Domain.csproj src/AuthService.Domain/
COPY src/AuthService.Persistence/AuthService.Persistence.csproj src/AuthService.Persistence/

RUN dotnet restore AuthService.sln

COPY . .
RUN dotnet publish src/AuthService.Api/AuthService.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "AuthService.Api.dll"]