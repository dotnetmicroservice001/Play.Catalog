# Play.Catalog - Play Economy 

This microservice is part of the **Play Economy** system and is responsible for handling catalog items available in the Play Economy System.


## Create and Publish Package 
```bash
version="1.0.2"
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
export version="1.0.3"
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
az acr login --name $acrname
docker push "$acrname.azurecr.io/play.catalog:$version"
```
## 🐳 Build & Push Docker Image (M2 Mac + AKS Compatible)
Build a multi-architecture image (ARM64 for local M2 Mac, AMD64 for AKS) and push to ACR:
```bash
version="1.0.3"
export GH_OWNER=dotnetmicroservice001
export GH_PAT="ghp_YourRealPATHere"
export acrname="playeconomy01acr"

az acr login --name $acrname
docker buildx build \
  --platform linux/amd64 \
  --secret id=GH_OWNER --secret id=GH_PAT \
  -t "$acrname.azurecr.io/play.catalog:$version" \
  --push .
```


## Creating Azure Managed Identity and granting it access to Key Vault Store
```bash
export appname=playeconomy-01
export namespace="catalog"
az identity create --resource-group $appname --name $namespace 

export IDENTITY_CLIENT_ID=$(az identity show -g "$appname" -n "$namespace" --query clientId -o tsv)
export SUBSCRIPTION_ID=$(az account show --query id -o tsv)

az role assignment create \
  --assignee "$IDENTITY_CLIENT_ID" \
  --role "Key Vault Secrets User" \
  --scope "/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$appname/providers/Microsoft.KeyVault/vaults/$appname"

```

---
