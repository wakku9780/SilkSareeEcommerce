# Use Ubuntu 20.04 base image for compatibility
FROM ubuntu:20.04

# Set environment variable to avoid interactive prompts during install
ENV DEBIAN_FRONTEND=noninteractive

# Install system dependencies including wkhtmltox dependencies and .NET prerequisites
RUN apt-get update && apt-get install -y \
    wget \
    xfonts-75dpi \
    xfonts-base \
    fontconfig \
    libjpeg-turbo8 \
    libxrender1 \
    libxtst6 \
    libssl1.1 \
    libssl-dev \
    ca-certificates \
    curl \
    apt-transport-https \
    gnupg \
    libicu66 \
    libkrb5-3 \
    liblttng-ust0 \
    libcurl4 \
    zlib1g \
    && rm -rf /var/lib/apt/lists/*

# Install wkhtmltox (PDF generator)
RUN wget https://github.com/wkhtmltopdf/packaging/releases/download/0.12.6-1/wkhtmltox_0.12.6-1.bionic_amd64.deb \
    && dpkg -i wkhtmltox_0.12.6-1.bionic_amd64.deb || apt-get install -f -y \
    && rm wkhtmltox_0.12.6-1.bionic_amd64.deb

# Install .NET 8.0 SDK
RUN wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh \
    && bash dotnet-install.sh --channel 8.0 --install-dir /usr/share/dotnet \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet \
    && rm dotnet-install.sh

# Set working directory inside the container
WORKDIR /app

# Copy project files to container
COPY . .

# Restore dependencies and publish app
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Expose port (optional, if your app listens on this port)
EXPOSE 8080

# Set entrypoint to run the app
ENTRYPOINT ["dotnet", "out/SilkSareeEcommerce.dll"]
