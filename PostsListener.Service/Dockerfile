FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["PostsListener.Service/PostsListener.Service.csproj", "PostsListener.Service/"]
RUN dotnet restore "PostsListener.Service/PostsListener.Service.csproj"
COPY . .
WORKDIR "/src/PostsListener.Service"
RUN dotnet build "PostsListener.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PostsListener.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PostsListener.Service.dll"]
