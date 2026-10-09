# Runtime image - ma tylko to, czego potrzeba do uruchomienia aplikacji
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

WORKDIR /app

EXPOSE 8080


# Build image - ma pełny .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["src/Booking.api/Booking.api.csproj", "src/Booking.api/"]
COPY ["src/Booking.Domain/Booking.Domain.csproj", "src/Booking.Domain/"]
COPY ["src/Booking.Infrastructure/Booking.Infrastructure.csproj", "src/Booking.Infrastructure/"]

RUN dotnet restore "src/Booking.api/Booking.api.csproj"

COPY . .

WORKDIR "/src/src/Booking.api"

RUN dotnet publish "Booking.api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# Final image
FROM base AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Booking.api.dll"]