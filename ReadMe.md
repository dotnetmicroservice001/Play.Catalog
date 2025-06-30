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
export GH_OWNER=dotnetmicroservice001
export GH_PAT="ghp_YourRealPATHere"
docker build --secret id=GH_OWNER --secret id=GH_PAT -t play.catalog:$version .
```

## Run Docker Image
```bash 
docker run -it --rm -p 5001:5000 --name identity -e MongoDbSettings__Host=mongo -e RabbitMQSettings__Host=rabbitmq --network playinfra_default play.catalog:$version
```

---
