FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "RealEstateManagement.Worker/RealEstateManagement.Worker.csproj"

RUN dotnet publish "RealEstateManagement.Worker/RealEstateManagement.Worker.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "RealEstateManagement.Worker.dll"]
