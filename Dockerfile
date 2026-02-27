# Stage 1: Imagem base com wkhtmltopdf (igual ao seu exemplo)
FROM ghcr.io/surnet/alpine-wkhtmltopdf:3.20.2-0.12.6-full AS wk

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
USER root

RUN apk add --no-cache tzdata \
    && cp /usr/share/zoneinfo/America/Sao_Paulo /etc/localtime \
    && echo "America/Sao_Paulo" > /etc/timezone

RUN apk add --no-cache \
    libstdc++ libx11 libxrender libxext libssl3 ca-certificates \
    fontconfig freetype ttf-dejavu ttf-droid ttf-freefont ttf-liberation \
    icu-libs icu-data-full \
    && fc-cache -f -v

COPY --from=wk /bin/wkhtmltopdf   /usr/bin/wkhtmltopdf
COPY --from=wk /bin/wkhtmltoimage /usr/bin/wkhtmltoimage
COPY --from=wk /lib/libwkhtmltox* /lib/

USER app
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Stage 2: Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["POC-Html-to-PDF.csproj", "./"]
RUN dotnet restore "POC-Html-to-PDF.csproj"

COPY . .
RUN dotnet build "POC-Html-to-PDF.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "POC-Html-to-PDF.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Imagem final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "POC-Html-to-PDF.dll"]
