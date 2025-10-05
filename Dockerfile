# Use official .NET 8 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CurrencyConverter.API/CurrencyConverter.Api.csproj", "CurrencyConverter.API/"]
COPY ["CurrencyConverter.Domain/CurrencyConverter.Domain.csproj", "CurrencyConverter.Domain/"]
COPY ["CurrencyConverter.Infrastructure/CurrencyConverter.Infrastructure.csproj", "CurrencyConverter.Infrastructure/"]
COPY ["CurrencyConverter.Model/CurrencyConverter.Model.csproj", "CurrencyConverter.Model/"]
COPY ["CurrencyConverter.Common/CurrencyConverter.Common.csproj", "CurrencyConverter.Common/"]
COPY . .
RUN dotnet restore "CurrencyConverter.API/CurrencyConverter.Api.csproj"
RUN dotnet publish "CurrencyConverter.API/CurrencyConverter.Api.csproj" -c Release -o /app/publish --no-restore

# Use official .NET 8 ASP.NET runtime image for final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
EXPOSE 5259
ENV ASPNETCORE_URLS=http://+:8080;http://+:5259
ENTRYPOINT ["dotnet", "CurrencyConverter.Api.dll"]
