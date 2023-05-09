# Check what account you are using
az account show
az login 

# Create a resource group
az group create --name "wj-azure-training" --location "CentralUS"

# Create Storage Account
az storage account  `
    --name "stazuremvapp" `
    --resource-group "wj-azure-training" `
    --sku "Standard_LRS"  `
    --location "CentralUS"

# Create Storage Container
az storage table create --account-name "stazuremvapp" --name "movies"
$end=(Get-Date).AddDays(2).ToString("yyyy-MM-ddTHH:mm:ssZ")
$sas=$(az storage account generate-sas `
    --permissions cdlruwap `
    --account-name "stazuremvapp" `
    --services t `
    --resource-types co `
    --expiry $end `
    -o tsv)

az storage account show-connection-string --name "stazuremvapp" --sas-token $sas -o tsv

az storage container create `
    --account-name "stazuremvapp" `
    --name "movies"

az storage blob upload `
    --account-name "stazuremvapp" `
    --container-name "movies" `
    --name "05.mp4" `
    --file ./videoplayback_lotr_3.mp4
