FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TelegramBotClient/TelegramBotClient.csproj", "TelegramBotClient/"]
COPY ["TelegramBotTgLibrary/TelegramBotTgLibrary.csproj", "TelegramBotTgLibrary/"]
RUN dotnet restore "TelegramBotClient/TelegramBotClient.csproj"
COPY . .
WORKDIR "/src/TelegramBotClient"
RUN dotnet build "TelegramBotClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelegramBotClient.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelegramBotClient.dll"]
