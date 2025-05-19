FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Install wkhtmltox dependencies and wkhtmltopdf package (Linux version)
RUN apt-get update && apt-get install -y \
    wget \
    xfonts-75dpi \
    xfonts-base \
    libjpeg62-turbo \
    libxrender1 \
    libxtst6 \
    libssl-dev \
    && wget https://github.com/wkhtmltopdf/packaging/releases/download/0.12.6-1/wkhtmltox_0.12.6-1.bionic_amd64.deb \
    && dpkg -i wkhtmltox_0.12.6-1.bionic_amd64.deb \
    && apt-get install -f -y \
    && rm wkhtmltox_0.12.6-1.bionic_amd64.deb

WORKDIR /app

COPY . .

ENTRYPOINT ["dotnet", "SilkSareeEcommerce.dll"]
