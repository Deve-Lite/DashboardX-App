<div align="center">
  
  <h1> Dashboard X </h1>
  <p> Let's make IT 😎 </p>

  <div>
    <a href="https://github.com/Deve-Lite/DashboardX-App/network/members">
      <img src="https://img.shields.io/github/forks/Deve-Lite/DashboardX-App" alt="forks" />
    </a>
    <a href="https://github.com/Deve-Lite/DashboardX-App/stargazers">
      <img src="https://img.shields.io/github/stars/Deve-Lite/DashboardX-App" alt="stars" />
    </a>
    <a href="https://github.com/Deve-Lite/DashboardX-App/issues/">
      <img src="https://img.shields.io/github/issues/Deve-Lite/DashboardX-App" alt="open issues" />
    </a>
    <a href="https://github.com/Deve-Lite/DashboardX-App/blob/master/LICENSE">
      <img src="https://img.shields.io/github/license/Deve-Lite/DashboardX-App" alt="license" />
    </a>
    <a href="https://github.com/Deve-Lite/DashboardX-App/actions/workflows/BuildDotnet.yml">
      <img src="https://github.com/Deve-Lite/DashboardX-App/actions/workflows/BuildDotnet.yml/badge.svg" alt="build" />
    </a>
    <a href="https://github.com/Deve-Lite/DashboardX-App/actions/workflows/BuildDocker.yml">
      <img src="https://github.com/Deve-Lite/DashboardX-App/actions/workflows/BuildDocker.yml/badge.svg" alt="docker" />
    </a>
  </div>
</div>

<br/>

### Built With

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white&style=flat)
![Blazor](https://img.shields.io/badge/Blazor-8B008B?style=for-the-badge&logo=blazor&logoColor=white&style=flat)

### About The Project

DashboardX: Your Gateway to the World of MQTT Devices.
DashboardX is an intuitive application that lets you manage and control devices using the MQTT protocol. It allows you to connect to multiple MQTT brokers (advising to use [HiveMQ](https://www.hivemq.com/)).

**Easy Device Addition**
- Select your device from the list or add your own by manually defining its specifics.
- Specify the controls you want to use to interact with the device:
  - Buttons: Perform actions with simple buttons.
  - State buttons: Track and change the state of your device.
  - Sliders: Precisely adjust values, e.g., lighting brightness.
- Assign each control an appropriate MQTT topic to communicate with the device.
- Optionally set the payload sent when interacting with the control.
-Intuitive Interface:

**DashboardX offers a user-friendly interface, so**
- Adding and managing devices is a breeze.
- Configuring controls and topics is clear and intuitive.
- The status of your devices is always visible at a glance.
- Start your IoT journey today!

DashboardX is an excellent solution for anyone who wants to build their own Internet of Things (IoT) solutions. Download the project from GitHub and see for yourself how easy and convenient it can be to control devices!

**Additional features**
- Support for multiple MQTT brokers
- Easy configuration of controls and topics
- Intuitive and user-friendly interface
- Real-time updates on device status


**DashboardX is the perfect tool for**
- DIYers and makers
- IoT developers
- Anyone who wants to build their own connected devices
  
#### Get started with DashboardX today!


### Api and Database

Api and database can be found in [DashboardX-API Repository](https://github.com/Deve-Lite/DashboardX-API).
Api is required to start the application.

### Local Setup

In order to run application SSL certificates will be required. 

##### OpenSSL
Some helper commands to ganerate SSL certs locally, **for development only**!

First of all, install `openssl`. Easily you can do it with [choco](https://chocolatey.org/install), and then run:
```
choco install openssl
```

##### DNS

**Windows**

Go to `C:\Windows\System32\drivers\etc\hosts`, **edit file as Admin** and add the following line:
```
127.0.0.1 dashboardx.local
127.0.0.1 dashboardx.docker
```

![image](https://github.com/Deve-Lite/DashboardX-Docs/assets/49869722/d51edaec-5462-4bf7-8a78-fed6fb398e62)


##### Steps
1. Create a private key for the CA (Certificate Authorities).
  ```
  openssl genrsa -aes256 -out DashboardX-RootCA.key 4096
  ```

2. Create Certificate of the CA
  ```
  openssl req -x509 -new -nodes -key DashboardX-RootCA.key -sha256 -days 1826 -out DashboardX-RootCA.crt -subj '/CN=DashboardX Root CA/C=PL/ST=Malopolska/L=Krakow/O=DashboardX'
  ```

3. Add the CA certificate to the trusted root certificates

  **Windows**
  
  Right click on the generated `.crt`, then select `Install`. Install it for all users and select `Trusted Root Certificate Authorities` from the list.

  ![image](https://github.com/Deve-Lite/DashboardX-Docs/assets/49869722/e3f0f20f-1224-41d1-985a-0fee520de3c4)

  **Ubuntu**

  ```
  sudo apt install -y ca-certificates
  sudo cp DashboardX-RootCA.crt /usr/local/share/ca-certificates
  sudo update-ca-certificates
  ```

4. Create a certificate for the webserver
  ```
 openssl req -new -nodes -out docker.csr -newkey rsa:4096 -keyout docker.key -subj '/CN=DashboardX/C=PL/ST=Malopolska/L=Krakow/O=DashboardX'
  ```

5. Create a `docker.v3.ext` file for SAN properties
  ```
  authorityKeyIdentifier=keyid,issuer
  basicConstraints=CA:FALSE
  keyUsage = digitalSignature, nonRepudiation, keyEncipherment, dataEncipherment
  subjectAltName = @alt_names
  [alt_names]
  DNS.1 = dashboardx.local
  DNS.2 = dashboardx.docker
  IP.1 = 0.0.0.0
  IP.2 = 127.0.0.1
  ```

6. Sign the certificate
  ```
  openssl x509 -req -in docker.csr -CA DashboardX-RootCA.crt -CAkey DashboardX-RootCA.key -CAcreateserial -out docker.crt -days 730 -sha256 -extfile docker.v3.ext
  ```

7. Put certificates in  ```src\Presentation``` folder

#### Docker 

Requires Certificates.

When using docker compose you should check 
- `API_URL` - and adjust to local api configuration

In order to run app:
- `docker-compose up -d`

Now you can acess app at `http://localhost:8080/`.

#### Visual Studio

Requires Certificates.

### Roadmap 

To see planned activities please see [Issues](https://github.com/Deve-Lite/DashboardX-App/issues).

### License

Distributed under the MIT License. See `LICENSE.txt` for more information.


### Contact

<div align="center">
  <a href="https://www.linkedin.com/in/lukasz-psp515-kolber/">
    <img src="https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white" alt="LinkedIn" />
  </a>
  <a href="https://twitter.com/psp515">
    <img src="https://img.shields.io/badge/Twitter-1DA1F2?style=for-the-badge&logo=twitter&logoColor=white" alt="Twitter" />
  </a>
</div>
