FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Scraper.MassTransit/Scraper.MassTransit.csproj", "Scraper.MassTransit/"]
RUN dotnet restore "Scraper.MassTransit/Scraper.MassTransit.csproj"
COPY . .
WORKDIR "/src/Scraper.MassTransit"
RUN dotnet build "Scraper.MassTransit.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Scraper.MassTransit.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY --from=build /src/Scraper.MassTransit/requirements.txt .
RUN apt-get update -y && apt-get install python3-pip -y 
RUN pip3 install --upgrade pip
RUN pip3 install -r "requirements.txt"

ENTRYPOINT ["dotnet", "Scraper.MassTransit.dll"]
