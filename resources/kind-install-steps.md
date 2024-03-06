Source used: https://sookocheff.com/post/kubernetes/local-kubernetes-development-with-kind/

- Create a kind cluster:

```bash
kind create cluster --config kind-config.yaml

kubectl cluster-info --context kind-kind
```
  
Load the images to kind:

```bash
kind load docker-image localhost:5000/backend:1 localhost:5000/frontend:1
```

- Install metrics server:

```bash
kubectl apply -f https://github.com/kubernetes-sigs/metrics-server/releases/latest/download/components.yaml

kubectl patch deployment metrics-server -n kube-system --type "json" -p '[{"op": "add", "path": "/spec/template/spec/containers/0/args/-", "value": "--kubelet-insecure-tls"}]'
```

- Install Nginx:

```bash
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/master/deploy/static/provider/kind/deploy.yaml

kubectl get pods -n ingress-nginx

??
kubectl delete -A ValidatingWebhookConfiguration ingress-nginx-admission
```

- Install Redis:

```bash
helm repo add bitnami https://charts.bitnami.com/bitnami

helm repo update

helm install redis bitnami/redis --set image.tag=6.2
```

- Install Conductor agent:

```bash
kubectl apply -f <ID>-agent-manifest.yaml
```

> Wait for Dapr to be installed.

- Install State Store Component:

```bash
kubectl apply -f statestore.yaml
```

- Deploy backend:

```bash
kubectl apply -f backend.yaml

kubectl rollout status deploy/backend

kubectl logs deployment/backend
```

- Deploy frontend:

```bash
kubectl apply -f frontend.yaml

kubectl rollout status deploy/frontend

kubectl logs deployment/frontend
```

- Check nodes/pods etc:

```bash
kubectl get nodes,pods,deployments,svc --all-namespaces
```

- Export cluster logs to file:

```bash
kubectl cluster-info dump > cluster-logs.txt
```

- Delete cluster:

```bash
kind delete cluster
```
