FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["src/SupplierIntegration.Worker/SupplierIntegration.Worker.csproj", "src/SupplierIntegration.Worker/"]
COPY ["src/Booking.Contracts/Booking.Contracts.csproj", "src/Booking.Contracts/"]

RUN dotnet restore "src/SupplierIntegration.Worker/SupplierIntegration.Worker.csproj"

COPY . .

WORKDIR "/src/src/SupplierIntegration.Worker"

RUN dotnet publish "SupplierIntegration.Worker.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SupplierIntegration.Worker.dll"]