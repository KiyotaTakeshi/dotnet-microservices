- deploy nginx ingress controller
	- [for Docker Desktop](https://kubernetes.github.io/ingress-nginx/deploy/#docker-desktop)

```shell
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.0.0/deploy/static/provider/cloud/deploy.yaml
```

```shell
kubectl get namespaces

$ kubectl get deployment -n ingress-nginx
NAME                       READY   UP-TO-DATE   AVAILABLE   AGE
ingress-nginx-controller   1/1     1            1           3m53

$ kubectl get pods -n ingress-nginx
NAME                                       READY   STATUS      RESTARTS   AGE
ingress-nginx-admission-create-7fvbk       0/1     Completed   0          3m11s
ingress-nginx-admission-patch-9tvrs        0/1     Completed   0          3m11s
ingress-nginx-controller-fd7bb8d66-tkqst   1/1     Running     0          3m11s

$ kubectl get services -n ingress-nginx
NAME                                 TYPE           CLUSTER-IP     EXTERNAL-IP   PORT(S)                      AGE
ingress-nginx-controller             LoadBalancer   10.100.99.33   localhost     80:32557/TCP,443:32293/TCP   4m21s
ingress-nginx-controller-admission   ClusterIP      10.106.4.99    <none>        443/TCP                      4m21s
```

- create secret

```shell
$ kubectl create secret generic mssql --from-literal=SA_PASSWORD="1qazxsw2"
secret/mssql created
```
