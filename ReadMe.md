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
export version="1.0.3"
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

---
