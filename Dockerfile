# Use the official IIS image
FROM mcr.microsoft.com/windows/servercore/iis:windowsservercore-ltsc2022

# Set working directory
WORKDIR C:\\setup

# Use PowerShell for shell
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Install Windows features
RUN dism /online /enable-feature /featurename:IIS-ASPNET45 /all; \
  dism /online /enable-feature /featurename:IIS-ApplicationDevelopment /all; \
  dism /online /enable-feature /featurename:IIS-NetFxExtensibility45 /all; \
  dism /online /enable-feature /featurename:IIS-ISAPIExtensions /all; \
  dism /online /enable-feature /featurename:IIS-ISAPIFilter /all; \
  dism /online /enable-feature /featurename:WCF-HTTP-Activation45 /all; \
  dism /online /enable-feature /featurename:NetFx4Extended-ASPNET45 /all; \
  dism /online /enable-feature /featurename:IIS-WebSockets /all; \
  dism /online /enable-feature /featurename:IIS-ApplicationInit /all; \
  # Install URL Rewrite
  Invoke-WebRequest -Uri https://download.microsoft.com/download/1/2/8/128E2E22-C1B9-44A4-BE2A-5859ED1D4592/rewrite_amd64_en-US.msi -OutFile rewrite_amd64.msi; \
  Start-Process msiexec.exe -ArgumentList '/i', 'rewrite_amd64.msi', '/quiet', '/norestart' -Wait; \
  Remove-Item -Path rewrite_amd64.msi

# Set the working directory to the IIS root
WORKDIR /inetpub/wwwroot

# Copy Docker config files
COPY src/Store/ . 
COPY src/Store/Web.Docker.config Web.config
COPY src/Store.UserAdmin/ ./Store.UserAdmin/.
COPY src/Store.UserAdmin/App.Docker.config Store.UserAdmin/bin/PDS.WITSMLstudio.Store.UserAdmin.exe.config

# Expose port 80
EXPOSE 80