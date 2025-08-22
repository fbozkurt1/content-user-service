# 1. ADIM: Uygulamanın çalışacağı temel (base) runtime imajını tanımla.
# Bu imaj, sadece uygulamayı çalıştırmak için gerekli bileşenleri içerir ve daha küçüktür.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. ADIM: Uygulamayı derlemek için SDK imajını kullan.
# Bu imaj, derleme araçlarını içerdiği için daha büyüktür.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/ContentService/ContentService.API/ContentService.API.csproj", "src/ContentService/ContentService.API/"]
# Proje referanslarını da kopyalamak iyi bir pratiktir, bu yüzden onları da ekledim.
COPY ["src/ContentService/ContentService.Application/ContentService.Application.csproj", "src/ContentService/ContentService.Application/"]
COPY ["src/ContentService/ContentService.Domain/ContentService.Domain.csproj", "src/ContentService/ContentService.Domain/"]
COPY ["src/ContentService/ContentService.Infrastructure/ContentService.Infrastructure.csproj", "src/ContentService/ContentService.Infrastructure/"]

RUN dotnet restore "src/ContentService/ContentService.API/ContentService.API.csproj"

# Proje dosyalarının geri kalanını kopyala
COPY . .
WORKDIR "/src/src/ContentService/ContentService.API"
RUN dotnet build "ContentService.API.csproj" -c Release -o /app/build

# 3. ADIM: Derlenen uygulamayı yayınla (publish).
FROM build AS publish
RUN dotnet publish "ContentService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. ADIM: Yayınlanan dosyaları en başta tanımladığımız küçük 'base' imajına kopyala.
# Artık 'FROM base' dediğimizde Docker hangi imajı kullanacağını biliyor.
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContentService.API.dll"]