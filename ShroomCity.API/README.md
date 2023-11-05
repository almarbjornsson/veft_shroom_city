
# How to deploy

0. **Prerequisites**

Login to Azure CLI, you need to have an Azure account and have the Azure CLI installed.
You also need to have an active subscription. 
```bash
az login
```

1. **Create a Resource Group in Azure**

```bash
 az group create --name shroomcityalmar --location northeurope
```

Creates a new Azure resource group.

2. **Create an Azure Container Registry (ACR)**

```bash
az acr create --resource-group shroomcityalmar --name shroomcitytemporary --sku Basic
```

Sets up an ACR for storing Docker images.

3. **Build and Tag Docker Image for ACR**
```bash
docker build . --tag shroomcitytemporary.azurecr.io/shroomcity:v1
```

Builds a Docker image and tags it for ACR.

4. **Push Docker Image to ACR**

```bash
docker push shroomcitytemporary.azurecr.io/shroomcity:v1
```

Pushes the local image to the ACR.

5. **Create an Azure Container Instance (ACI)**

```bash
az container create --resource-group shroomcityalmar --name shroomcity --image shroomcitytemporary.azurecr.io/shroomcity:v1 --dns-name-label sc-api --ports 80
```

Deploys the image to an ACI, making it publicly accessible.

6. **Check the Deployed Container Instance**

```bash
az container show --resource-group shroomcityalmar --name shroomcity --query "{FQDN:ipAddress.fqdn,ProvisioningState:provisioningState}" --out table
```

Retrieves deployment details of the container instance. I used this to check if it deployed successfully.

7. **Access the Swagger UI**

Navigate to the deployed application's Swagger UI: [http://sc-api.northeurope.azurecontainer.io/swagger/index.html](http://sc-api.northeurope.azurecontainer.io/swagger/index.html)

The API is now publicly available on this URI:  http://sc-api.northeurope.azurecontainer.io/


8. **Update Web API**

First we delete the old instance
```bash
az container delete --resource-group shroomcityalmar --name shroomcity
```

Then we deploy the newer version.  I had built, tagged, and pushed a version V2 of the API to the ACR before this.
```bash
az container create --resource-group shroomcityalmar --name shroomcity --image shroomcitytemporary.azurecr.io/shroomcity:v2 --dns-name-label sc-api --ports 80
```



### Notes
This is a very basic and insecure setup.
- No HTTPS + Certificate
- No logging or monitoring
- No CI/CD pipeline
- Database is not necessarily hosted in the same region as the API (ElephantSQL)
