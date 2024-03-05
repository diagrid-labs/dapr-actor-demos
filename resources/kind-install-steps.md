Source used: https://sookocheff.com/post/kubernetes/local-kubernetes-development-with-kind/

- Install local docker registry:

```bash
docker run -d -p 5000:5000 --restart=always --name registry registry:2.7
```

- Push local images:

```bash
docker push localhost:5000/backend:1
docker push localhost:5000/frontend:1
```

- Create a kind cluster:

```bash
kind create cluster --config kind-config.yaml

kubectl cluster-info --context kind-kind
```

- Connect kind cluster to local docker registry:

```bash
docker network connect "kind" "registry"

docker network inspect kind
```

- Install metrics server:

```bash
kubectl apply -f https://github.com/kubernetes-sigs/metrics-server/releases/latest/download/components.yaml

kubectl patch deployment metrics-server -n kube-system --type "json" -p '[{"op": "add", "path": "/spec/template/spec/containers/0/args/-", "value": "--kubelet-insecure-tls"}]'
```

- Install Nginx:

```bash
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/master/deploy/static/provider/kind/deploy.yaml
```

- Install Conductor agent:

```bash
kubectl apply -f <ID>-agent-manifest.yaml
```

> Wait for Dapr to be installed.

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
