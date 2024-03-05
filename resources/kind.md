Source used: https://sookocheff.com/post/kubernetes/local-kubernetes-development-with-kind/

- Install local docker registry:

```bash
docker run -d -p 5000:5000 --restart=always --name registry registry:2.7
```

- Create a kind cluster:

```bash
kind create cluster --config kind-config.yaml
```

- Connect kind cluster to local docker registry:

```bash
docker network connect "kind" "registry"
```

- Install metrics server:

```bash
kubectl apply -f https://github.com/kubernetes-sigs/metrics-server/releases/latest/download/components.yaml
```

```bash
kubectl patch deployment metrics-server -n kube-system --type "json" -p '[{"op": "add", "path": "/spec/template/spec/containers/0/args/-", "value": "--kubelet-insecure-tls"}]
```

- Install Conductor agent:

```bash
kubectl apply -f ***-agent-manifest.yaml
```

- Deploy backend:

```bash
kubectl apply -f backend.yaml
```

- Deploy frontend:

```bash
kubectl apply -f frontend.yaml
```

- Delete cluster:

```bash
kind delete cluster
```
