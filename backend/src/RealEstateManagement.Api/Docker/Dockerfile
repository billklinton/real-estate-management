FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "RealEstateManagement.Api/RealEstateManagement.Api.csproj"

RUN dotnet publish "RealEstateManagement.Api/RealEstateManagement.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "RealEstateManagement.Api.dll"]
