
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.2-alpine3.11 AS base
MAINTAINER DevDotNet.Org <anton@devdotnet.org>
LABEL maintainer="DevDotNet.Org <anton@devdotnet.org>"

WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

WORKDIR /src
COPY ["ApidotNetWallet/ApidotNetWallet.csproj", "ApidotNetWallet/"]
RUN dotnet restore "ApidotNetWallet/ApidotNetWallet.csproj"
COPY . .
WORKDIR "/src/ApidotNetWallet"
RUN dotnet build "ApidotNetWallet.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApidotNetWallet.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
VOLUME ["/app/config"]
ENTRYPOINT ["dotnet", "ApidotNetWallet.dll"]