FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["src/MockSupplier.Api/MockSupplier.Api.csproj", "src/MockSupplier.Api/"]

RUN dotnet restore "src/MockSupplier.Api/MockSupplier.Api.csproj"

COPY . .

WORKDIR "/src/src/MockSupplier.Api"

RUN dotnet publish "MockSupplier.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "MockSupplier.Api.dll"]