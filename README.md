# Fair DevOps Assessment - Proje README

## Proje Özeti
Bu proje, bir ASP.NET Core 8.0 Minimal API uygulamasının Docker, Kubernetes (Helm), Prometheus-Grafana monitoring, Horizontal Pod Autoscaler (HPA) ve Azure DevOps CI/CD pipeline ile uçtan uca DevOps süreçlerini kapsar.

---

## İçindekiler
- [Gereksinimler](#gereksinimler)
- [Proje Yapısı](#proje-yapısı)
- [Kurulum ve Çalıştırma](#kurulum-ve-çalıştırma)
  - [1. Uygulama Geliştirme ve Docker](#1-uygulama-geliştirme-ve-docker)
  - [2. Kubernetes ve Helm ile Deploy](#2-kubernetes-ve-helm-ile-deploy)
  - [3. Monitoring (Prometheus + Grafana)](#3-monitoring-prometheus--grafana)
  - [4. Horizontal Pod Autoscaler (HPA)](#4-horizontal-pod-autoscaler-hpa)
  - [5. Azure DevOps Pipeline](#5-azure-devops-pipeline)
- [Önemli Notlar ve Troubleshooting](#önemli-notlar-ve-troubleshooting)
- [Kaynaklar](#kaynaklar)

---

## Gereksinimler
- Docker Desktop (Kubernetes etkin)
- .NET 8 SDK
- Helm CLI
- kubectl CLI
- Azure DevOps hesabı
- DockerHub hesabı (veya başka bir container registry)
- (Opsiyonel) curl, ab, hey gibi yük test araçları

---

## Proje Yapısı
```
FairDevopsAPI/
  ├── Program.cs
  ├── FairDevopsAPI.csproj
  ├── Dockerfile
  ├── helm/
  │   ├── templates/
  │   │   ├── deployment.yaml
  │   │   ├── service.yaml
  │   │   ├── hpa.yaml
  │   │   └── ...
  │   └── values.yaml
  ├── monitoring/
  │   ├── templates/
  │   │   ├── grafana-deployment.yaml
  │   │   ├── grafana-service.yaml
  │   │   ├── grafana-pvc.yaml
  │   │   ├── prometheus-configmap.yaml
  │   │   ├── prometheus-deployment.yaml
  │   │   └── prometheus-service.yaml
  │   ├── Chart.yaml
  │   └── values.yaml
  ├── metrics-server/
  │   └── components.yaml
  ├── deploy-monitoring.sh
  ├── azure-pipelines.yml
  └── README.md
```

---

## Kurulum ve Çalıştırma

### 1. Uygulama Geliştirme ve Docker
- Uygulama .NET 8 minimal API ile yazıldı.
- CRUD endpoint’leri, health check ve Swagger UI her ortamda açık.
- Dockerfile ile multi-stage build ve port 8080 expose edildi.

**Docker ile çalıştırmak için:**
```bash
cd FairDevopsAPI
dotnet build
docker build -t fairdevops-api:latest .
docker run -p 30081:8080 fairdevops-api:latest
```
Swagger: [http://localhost:30081/swagger](http://localhost:30081/swagger)

---

### 2. Kubernetes ve Helm ile Deploy

**Helm ile deploy:**
```bash
cd FairDevopsAPI
helm install fairdevops-api ./helm
```
Servis NodePort: 30081 (API ve Swagger)

**Kaynakları kontrol et:**
```bash
kubectl get pods
kubectl get svc
```

---

### 3. Monitoring (Prometheus + Grafana)

**Monitoring stack’i deploy etmek için:**
```bash
./deploy-monitoring.sh
```
- Prometheus: [http://localhost:30090](http://localhost:30090)
- Grafana: [http://localhost:30091](http://localhost:30091) (admin/admin123)

**Grafana dashboardları ve ayarları kalıcıdır (PVC ile).**
- Dashboard provisioning ile otomatik olarak hazır dashboardlar yüklenebilir.
- Prometheus, uygulamanın /metrics endpoint’inden veri toplar.

---

### 4. Horizontal Pod Autoscaler (HPA)

**metrics-server kurulumu:**
```bash
kubectl apply -f FairDevopsAPI/metrics-server/components.yaml
```
- Bu dosya Docker Desktop ve local Kubernetes için özel olarak ayarlanmıştır (image: v0.6.3, port: 4443, --kubelet-insecure-tls).
- HPA’nın çalışması için metrics-server zorunludur.

**HPA’yı uygulamak için:**
```bash
kubectl apply -f FairDevopsAPI/helm/templates/hpa.yaml
```
**Durumu kontrol et:**
```bash
kubectl get hpa
kubectl describe hpa fairdevops-api-hpa
```
**Pod metriklerini görmek için:**
```bash
kubectl top pods
```

---

### 5. Azure DevOps Pipeline

- `azure-pipelines.yml` ile CI/CD süreci:
  - .NET build, publish, Docker image build & push, Helm deploy adımları.
- Pipeline’da Docker, kubectl, Helm yüklü self-hosted agent kullanıldı.
- Secret yönetimi için Azure DevOps variable groups veya secrets kullanılabilir.

---

## Önemli Notlar ve Troubleshooting

- **metrics-server** olmadan HPA çalışmaz!  Kurulumdan sonra `kubectl top pods` ile metriklerin geldiğini kontrol edilmeli.
- **Prometheus ve Grafana** için PVC tanımlı, dashboardlar ve ayarlar pod silinse de kalıcıdır.
- **Monitoring dashboardları** otomatik provisioning ile veya Grafana arayüzünden import ile eklenebilir.
- **HPA** için deployment’ta resource requests/limits tanımlı olmalı.
- **Her ortamda çalışması için** metrics-server/components.yaml olmalı ve terminalde deploy-monitoring.sh yürütülmelidir
- **metrics-server/components.yaml** dosyası Docker Desktop ve local Kubernetes için optimize edilmiştir. Cloud ortamında farklı image veya port ayarları gerekebilir.



## Terminalde Yapılanlar
- Uygulama build ve Docker image oluşturma
- Helm ile deploy
- Monitoring stack deploy (Prometheus + Grafana)
- metrics-server kurulumu ve HPA uygulama
- PVC ile kalıcı storage sağlama
- HPA ve monitoring için troubleshooting ve log analizi
- Grafana dashboardlarını kalıcı ve otomatik provisioning ile yönetme

---

--test