# ============================
# 1. BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# copy project files and restore dependencies
COPY *.csproj .
RUN dotnet restore

# copy everything and build
COPY . .
RUN dotnet publish -c Release -o out

# ============================
# 2. RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Ecommerce.Api.dll"]