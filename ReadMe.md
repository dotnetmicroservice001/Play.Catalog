# Play.Catalog - Play Economy 

This microservice is part of the **Play Economy** system and is responsible for handling catalog items available in the Play Economy System.


## Create and Publish Package 
```bash
owner="dotnetmicroservice001"
gh_pat="[YOUR_PERSONAL_ACCESS_TOKEN]"

dotnet pack src/Play.Catalog.Contracts --configuration Release \
  -p:PackageVersion="$version" \
  -p:RepositoryUrl="https://github.com/$owner/Play.Catalog" \
  -o ../Packages
  
dotnet nuget push ../Packages/Play.Catalog.Contracts.$version.nupkg --api-key $gh_pat \
--source "github"
```
## Build a Docker Image
```bash
export GH_OWNER=dotnetmicroservice001
export GH_PAT="ghp_YourRealPATHere"
export acrname="playeconomy01acr"
docker build --secret id=GH_OWNER --secret id=GH_PAT -t "$acrname.azurecr.io/play.catalog:$version" .
```

## Run Docker Image
```bash 
export cosmosDbConnString="conn string here"
export serviceBusConnString="conn string here"
docker run -it --rm \
  -p 5001:5000 \
  --name catalog \
  -e MongoDbSettings__ConnectionString=$cosmosDbConnString \
  -e ServiceBusSettings__ConnectionString=$serviceBusConnString \
  -e ServiceSettings__MessageBroker="SERVICEBUS" \
  play.catalog:$version
```

## Publishing Docker Image
```bash 
az acr login --name $appname
docker push "$appname.azurecr.io/play.catalog:$version"
```

## 🐳 Build & Push Docker Image (M2 Mac + AKS Compatible)
Build a multi-architecture image (ARM64 for local M2 Mac, AMD64 for AKS) and push to ACR:
```bash
version="1.0.0"
export GH_OWNER=dotnetmicroservice001
export GH_PAT="ghp_YourRealPATHere"
export ACR="acrpfpos"

az acr login --name $ACR
docker buildx build \
  --platform linux/amd64 \
  --secret id=GH_OWNER --secret id=GH_PAT \
  -t "$ACR.azurecr.io/play.catalog:$version" \
  --push .
```

## Create Kubernetes namespace
```bash 
export namespace="catalog"
kubectl create namespace $namespace 
```

## Creating Azure Managed Identity and granting it access to Key Vault Store
```bash
export RG=rg-playground
export AKS="aks-playground"
export KV="kv-pf-pos"
az identity create --resource-group $RG --name $namespace 

export IDENTITY_CLIENT_ID=$(az identity show -g "$RG" -n "$namespace" --query clientId -o tsv)
export SUBSCRIPTION_ID=$(az account show --query id -o tsv)

az role assignment create \
  --assignee "$IDENTITY_CLIENT_ID" \
  --role "Key Vault Secrets User" \
  --scope "/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RG/providers/Microsoft.KeyVault/vaults/$KV"
```

## Establish the related Identity Credential
```bash
export AKS_OIDC_ISSUER="$(az aks show -n $AKS -g $RG --query "oidcIssuerProfile.issuerUrl" -otsv)"
az identity federated-credential create --name ${namespace} --identity-name "${namespace}" --resource-group "${RG}" --issuer "${AKS_OIDC_ISSUER}" --subject system:serviceaccount:"${namespace}":"${namespace}-serviceaccount" --audience api://AzureADTokenExchange
```

## install helm chart
```bash 
helmUser="00000000-0000-0000-0000-000000000000"
helmPassword=$(az acr login --name $ACR --expose-token --output tsv --query accessToken)
helm registry login $ACR.azurecr.io --username $helmUser --password $helmPassword 

chartVersion="0.1.2"
helm upgrade "pe-$namespace-service" oci://$ACR.azurecr.io/helm/playflow-microservice --version $chartVersion -f ./helm/values.yaml -n $namespace --install
```
---
## Required repository secrets for github workflow
GH_PAT -> profile - settings - developer settings - PAT with repo and read access
AZURE_CLIENT_ID -> from ad app registration
AZURE_SUBSCIPRTION_ID -> from azure subscription portal
AZURE_TENANT_ID -> from microsoft entra id