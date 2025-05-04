## WITSML

This repository provides a functional WITSML server, originally developed by PDS and
now restored with a proper documentation. It is now updated and maintained to support
the community and industry with a reliable test WITSML server implementation.

### Setup Guide

These setup steps have been tested on both Windows 10 and Windows 11. Follow these steps to set up and build the project:

1. **Install Prerequisites**:
    - Download **Visual Studio 2022** (tested with v17+) [here](https://visualstudio.microsoft.com/) and install.
    - Add `msbuild` to your user's PATH environment variables:
      ```sh
      # Example path
      C:\Program Files\Microsoft Visual Studio\2022\<VERSION>\Msbuild\Current\Bin
      # For instance
      C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin
      ```
      ![environment-variable](img/environment-variable.png)
    - Download **MongoDB** [here](https://www.mongodb.com/try/download/community) (tested with v8+), install and ensure it is running on port `27017`.
    - Download **MongoDB Compass** [here](https://www.mongodb.com/try/download/compass) (tested with v1.46+), install and create a database named `WitsmlStore`.
      ![mongodb-compass](img/mongo-db-compass.png)
    - Download IIS **URL Rewrite** [here](https://www.iis.net/downloads/microsoft/url-rewrite) and install.

1. **Prepare Environment**:
    - Type "Turn Windows features on or off" in Start and open.
    - Turn on Internet Information Services (IIS).

      ![windows-feature-iis](img/windows-feature-iis.png)
    - Turn on some Application Development Features.

      ![windows-feature-application-development-features](img/windows-feature-application-development-features.png)
    - Turn on HTTP Activation.

      ![windows-feature-http-activation](img/windows-feature-http-activation.png)

2. **Build the Project**:
    - Run Visual Studio as Administrator.
    - Make sure to finish installing your project components.
      ![visual-studio-install-components](img/visual-studio-install-components.png)
    - Restore NuGet packages.
      ![visual-studio-install-nuget-packages](img/visual-studio-install-nuget-packages.png)
    - Select "Release" and run "Build > Rebuild Solution":
      ![visual-studio-build-release](img/visual-studio-build-release.png)

3. **Configure IIS Server**:
    - Open IIS Manager and Add Website by right-click on Sites.
      ![iis-add-site](img/iis-add-site.png)
    - Add any site name, set physical path to `<Project-Path>\src\Store` and set your desired port.
      ![iis-add-site2](img/iis-add-site2.png)
    - Add permissions to `<Project-Path>\src\Store` (by right-click on the folder) to IUSR.
      ![iis-add-permission](img/iis-add-permission.png)

5. **Add a User**:
    - Open PowerShell in the `.\src\Store.UserAdmin\bin` folder.
    - Add a user with your desired credentials:
      ```sh
      .\PDS.WITSMLstudio.Store.UserAdmin.exe add -u <Your-Username> -p <Your-Password> -e <Your-Email>
      ```
      ![powershell-create-user](img/powershell-create-user.png)

6. **Connect to the Server**:
    - Access the server at `http://localhost:<IIS-Site-Port>/api/soap` using:
      - **Username**: `<Your-Username>`
      - **Password**: `<Your-Password>`
    - Use [PDS WITSMLstudio Desktop](https://witsml.pds.technology/studio/) if no client is available.
      ![witsmlstudio-connection](img/witsmlstudio-connection.png)

By completing these steps, the project should be successfully set up and ready for use.

### Containerization Guide

> **Note**: Ensure the project is built successfully before proceeding with containerization. MongoDB is not required if you want to containerize the project.

If you want to containerize the server, follow these steps:

1. **Enable Windows Features for Containers**:
  - Open PowerShell as Administrator and run the following command to enable the required Windows features:
    ```sh
    Enable-WindowsOptionalFeature -Online -FeatureName $("Microsoft-Hyper-V", "Containers") -All
    ```
  - Restart your system if required.

2. **Install and Configure Docker**:
  - Download [Docker Desktop](https://www.docker.com/products/docker-desktop/) (tested on v4.41+) and install.
  - Ensure Docker is running in **Windows Containers** mode. You can switch to Windows Containers by right-clicking the Docker icon in the system tray and selecting "Switch to Windows Containers."

3. **Build and Run the Docker Container**:
  - Open a terminal in this directory and run the following command to build and start the container:
    ```sh
    docker-compose up --build
    ```
  - Once the container is running, the server will be accessible at `http://localhost:8080`.

4. **Add a User**:
  - Open container PowerShell by running the following command:
    ```sh
    docker exec -it <Your-Container-Name> powershell
    ```
  - Add a user with your desired credentials:
    ```sh
    .\Store.UserAdmin\bin\PDS.WITSMLstudio.Store.UserAdmin.exe add -u <Your-Username> -p <Your-Password> -e <Your-Email>
    ```
    ![powershell-create-user](img/powershell-create-user.png)

---
### Copyright and License
Copyright &copy; 2018 PDS Americas LLC

Released under the PDS Open Source WITSML™ Product License Agreement
http://www.pds.group/WITSMLstudio/OpenSource/ProductLicenseAgreement

---

### Export Compliance

This source code makes use of cryptographic software:
- SSL/TLS is optionally used to secure web communications

The country in which you currently reside may have restrictions on the import, possession,
use, and/or re-export to another country, of encryption software.  BEFORE using any
encryption software, please check your country's laws, regulations and policies concerning
the import, possession, or use, and re-export of encryption software, to see if this is
permitted.  See <http://www.wassenaar.org/> for more information.

The U.S. Government Department of Commerce, Bureau of Industry and Security (BIS), has
classified this source code as Export Control Classification Number (ECCN) 5D002.c.1, which
includes information security software using or performing cryptographic functions with
symmetric and/or asymmetric algorithms.

This source code is published here:
> https://github.com/pds-technology/witsml

In accordance with US Export Administration Regulations (EAR) Section 742.15(b), this
source code is not subject to EAR:
 - This source code has been made publicly available in accordance with EAR Section
   734.3(b)(3)(i) by publishing it in accordance with EAR Section 734.7(a)(4) at the above
   URL.
 - The BIS and the ENC Encryption Request Coordinator have been notified via e-mail of this
   URL.

