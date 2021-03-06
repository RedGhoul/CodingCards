FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build
WORKDIR /src/CodingCards/
COPY ["CodingCards.csproj", "/src/CodingCards/"]
RUN dotnet restore "CodingCards.csproj"
COPY . .
WORKDIR "/src/CodingCards/"
RUN dotnet build "CodingCards.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CodingCards.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodingCards.dll"]
