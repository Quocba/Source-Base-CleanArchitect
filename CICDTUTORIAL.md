# 🚀 Hướng dẫn CI/CD ASP.NET Core + Docker trên Windows Server (Self‑hosted Runner)

Tài liệu này tổng hợp **toàn bộ các bước từ đầu đến cuối** để triển khai (deploy) một dự án **ASP.NET Core** lên **VPS Windows Server** bằng **Docker + GitHub Actions (Self‑hosted Runner)**, trỏ **domain** và chạy **production‑ready**.

---

## 🧭 Kiến trúc tổng thể

```
GitHub (push code)
   ↓
GitHub Actions (self‑hosted runner trên VPS Windows)
   ↓
Docker build & run container
   ↓
Nginx (reverse proxy)
   ↓
Domain (api.yourdomain.com)
```

---

## 1️⃣ Chuẩn bị môi trường trên VPS Windows Server

### Yêu cầu
- Windows Server 2019 / 2022
- Docker Engine (Windows containers)
- Git
- Domain đã mua

---

## 2️⃣ Cài GitHub Actions Self‑hosted Runner

### 2.1 Tải runner

```powershell
mkdir C:\actions-runner
cd C:\actions-runner

Invoke-WebRequest -Uri https://github.com/actions/runner/releases/download/v2.330.0/actions-runner-win-x64-2.330.0.zip -OutFile actions-runner.zip
Expand-Archive actions-runner.zip
```

### 2.2 Lấy token

GitHub → Repository → **Settings → Actions → Runners → New self-hosted runner** → copy **registration token**

### 2.3 Register runner

```powershell
./config.cmd
```

- Repo URL: `https://github.com/<owner>/<repo>`
- Token: (token vừa lấy)
- Runner name: Enter
- Labels: Enter
- Work folder: Enter

### 2.4 Chạy runner

```powershell
./run.cmd
```

➡️ Khi thấy `Listening for Jobs` là thành công.

---

## 3️⃣ Dockerfile chuẩn cho ASP.NET Core (Windows)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0-windowsservercore-ltsc2019 AS base
WORKDIR /app
EXPOSE 8081
EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:9.0-windowsservercore-ltsc2019 AS build
WORKDIR /src

COPY ["BaseAPI/API.csproj", "BaseAPI/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

RUN dotnet restore "BaseAPI/API.csproj"

COPY . .
WORKDIR "/src/BaseAPI"
RUN dotnet publish "API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]
```

---

## 4️⃣ Fix lỗi DI & appsettings (rất quan trọng)

### ❌ Sai (gây crash container)

```csharp
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddSingleton(jwtSettings);
```

### ✅ Đúng (chuẩn production)

```csharp
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
```

### ⚠️ Không build ServiceProvider trong Program.cs

❌
```csharp
builder.Services.BuildServiceProvider();
```

---

## 5️⃣ Ép ASP.NET Core listen đúng port (8081 / 8082)

Trong `Program.cs`:

```csharp
builder.WebHost.UseUrls("http://+:8081", "http://+:8082");
```

---

## 6️⃣ Copy appsettings vào Docker publish

Trong `BaseAPI/API.csproj`:

```xml
<ItemGroup>
  <None Update="appsettings.json">
    <CopyToPublishDirectory>Always</CopyToPublishDirectory>
  </None>
  <None Update="appsettings.Production.json">
    <CopyToPublishDirectory>Always</CopyToPublishDirectory>
  </None>
</ItemGroup>
```

---

## 7️⃣ Build & chạy container

```powershell
docker build -t baseapi:latest .

docker rm baseapi 2>$null

docker run -d `
 --name baseapi `
 -p 8081:8081 `
 -p 8082:8082 `
 baseapi:latest
```

Kiểm tra:
```powershell
docker logs baseapi
docker ps
```

---

## 8️⃣ GitHub Actions CI/CD workflow

📁 `.github/workflows/deploy.yml`

```yaml
name: Deploy BaseAPI

on:
  push:
    branches: [ main ]

jobs:
  deploy:
    runs-on: self-hosted

    steps:
      - uses: actions/checkout@v4

      - name: Build image
        run: docker build -t baseapi:latest .

      - name: Deploy
        shell: powershell
        run: |
          docker stop baseapi 2>$null
          docker rm baseapi 2>$null
          docker run -d `
            --name baseapi `
            -p 8081:8081 `
            -p 8082:8082 `
            baseapi:latest
```

➡️ Push code là **auto deploy**.

---

## 9️⃣ Trỏ domain

### 9.1 DNS

Tạo **A record**:

| Type | Name | Value |
|---|---|---|
| A | api | IP_VPS |

Ví dụ:
```
api.ngocdai.com → 103.xxx.xxx.xxx
```

---

## 🔟 Nginx reverse proxy (chạy bằng Docker)

### 10.1 Tạo config

`C:\nginx\conf\default.conf`

```nginx
server {
    listen 80;
    server_name api.ngocdai.com;

    location / {
        proxy_pass http://host.docker.internal:8081;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    }
}
```

### 10.2 Chạy Nginx

```powershell
docker run -d `
 --name nginx `
 -p 80:80 `
 -v C:\nginx\conf:/etc/nginx/conf.d `
 nginx:latest
```

Mở firewall:
```powershell
New-NetFirewallRule -DisplayName "HTTP 80" -Direction Inbound -Protocol TCP -LocalPort 80 -Action Allow
```

---

## 11️⃣ HTTPS

### Nếu dùng Cloudflare
- Bật **Proxy (mây cam)**
- SSL mode: **Full**
➡️ Không cần cài cert trong VPS

---

## ✅ Checklist production-ready

- [x] Docker build OK
- [x] Container chạy `Up`
- [x] ASP.NET listen 8081 / 8082
- [x] CI/CD auto deploy
- [x] RabbitMQ / Redis hoạt động
- [x] Domain trỏ thành công
- [x] Reverse proxy sạch

---

## 🎉 KẾT LUẬN

Bạn đã có **pipeline CI/CD hoàn chỉnh trên Windows Server**:
- Không IIS
- Không port xấu
- Không manual deploy
- Sẵn sàng mở rộng production

---

📌 Tài liệu này có thể dùng làm **README nội bộ / onboarding / handover**.

