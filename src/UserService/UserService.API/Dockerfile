# 1. ADIM: Uygulamanın çalışacağı temel (base) runtime imajını tanımla.
# Bu imaj, sadece uygulamayı çalıştırmak için gerekli bileşenleri içerir ve daha küçüktür.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. ADIM: Uygulamayı derlemek için SDK imajını kullan.
# Bu imaj, derleme araçlarını içerdiği için daha büyüktür.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/UserService/UserService.API/UserService.API.csproj", "src/UserService/UserService.API/"]
# Proje referanslarını da kopyalamak iyi bir pratiktir, bu yüzden onları da ekledim.
COPY ["src/UserService/UserService.Application/UserService.Application.csproj", "src/UserService/UserService.Application/"]
COPY ["src/UserService/UserService.Domain/UserService.Domain.csproj", "src/UserService/UserService.Domain/"]
COPY ["src/UserService/UserService.Infrastructure/UserService.Infrastructure.csproj", "src/UserService/UserService.Infrastructure/"]

RUN dotnet restore "src/UserService/UserService.API/UserService.API.csproj"

# Proje dosyalarının geri kalanını kopyala
COPY . .
WORKDIR "/src/src/UserService/UserService.API"
RUN dotnet build "UserService.API.csproj" -c Release -o /app/build

# 3. ADIM: Derlenen uygulamayı yayınla (publish).
FROM build AS publish
RUN dotnet publish "UserService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. ADIM: Yayınlanan dosyaları en başta tanımladığımız küçük 'base' imajına kopyala.
# Artık 'FROM base' dediğimizde Docker hangi imajı kullanacağını biliyor.
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.API.dll"]