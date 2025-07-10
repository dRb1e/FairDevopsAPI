#!/bin/bash

echo "🚀 Deploying Fair DevOps API Monitoring Stack..."

# Deploy Prometheus
echo "📊 Deploying Prometheus..."
kubectl apply -f monitoring/templates/prometheus-configmap.yaml
kubectl apply -f monitoring/templates/prometheus-deployment.yaml
kubectl apply -f monitoring/templates/prometheus-service.yaml

# Deploy Grafana
echo "📈 Deploying Grafana..."
kubectl apply -f monitoring/templates/grafana-deployment.yaml
kubectl apply -f monitoring/templates/grafana-service.yaml
kubectl apply -f monitoring/templates/grafana-pvc.yaml

# Wait for pods to be ready
echo "⏳ Waiting for monitoring pods to be ready..."
kubectl wait --for=condition=ready pod -l app=prometheus --timeout=300s
kubectl wait --for=condition=ready pod -l app=grafana --timeout=300s
# Deployment metrics-server
echo "Deploying metrics-server..."
kubectl apply -f FairDevopsAPI/metrics-server/components.yaml

echo "Monitoring stack deployed successfully!"
echo ""
echo "Access URLs:"
echo "   Prometheus: http://localhost:30090"
echo "   Grafana:    http://localhost:30091 (admin/admin123)"
echo ""
echo "Check pod status:"
kubectl get pods -l app=prometheus
kubectl get pods -l app=grafana