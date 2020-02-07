# API Proxy

Creates forwarding API proxy.

Mutiple proxy routes can be setup in appsettings.json

Assumption is that all backend APIs use basic auth

```json
"ProxyConfig": {
    "BasicAuth": {
      "User": "admin",
      "Password": "password"
    },
    "ProxyRoutes": [
      {
        "External": "/api",
        "ForwardTo": "http://localhost:8080"
      },
      {
        "External": "/api2",
        "ForwardTo": "http://localhost:8080"
      }
    ]
  }
```

## Setup environment and run

- [Install dotnet core](https://dotnet.microsoft.com/download)

```
dotnet restore
dotnet run
```
