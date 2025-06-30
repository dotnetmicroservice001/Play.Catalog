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

---
