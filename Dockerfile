# Use Ubuntu 20.04 base image for compatibility
FROM ubuntu:20.04

# Set environment variable to avoid interactive prompts during install
ENV DEBIAN_FRONTEND=noninteractive

# Install system dependencies including wkhtmltox dependencies
RUN apt-get update && apt-get install -y \
    wget \
    xfonts-75dpi \
    xfonts-base \
    fontconfig \
    libjpeg-turbo8 \
    libjpeg62-turbo \
    libxrender1 \
    libxtst6 \
    libssl1.1 \
    libssl-dev \
    ca-certificates \
    curl \
    apt-transport-https \
    gnupg \
    && wget https://github.com/wkhtmltopdf/packaging/releases/download/0.12.6-1/wkhtmltox_0.12.6-1.bionic_amd64.deb \
    && dpkg -i wkhtmltox_0.12.6-1.bionic_amd64.deb || apt-get install -f -y \
    && rm wkhtmltox_0.12.6-1.bionic_amd64.deb \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Install .NET SDK (change version as per your project)
RUN wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh \
    && bash dotnet-install.sh --channel 7.0 --install-dir /usr/share/dotnet \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

# Set working directory
WORKDIR /app

# Copy your project files
COPY . .

# Restore .NET dependencies and build your app
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Set the entrypoint to run your app
ENTRYPOINT ["dotnet", "out/SilkSareeEcommerce.dll"]
