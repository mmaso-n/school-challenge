<#
    This script creates the tables required for the SchoolChallenge Project.
    
    TODO: Perform storage account creation using ARM templates, output the account key and send it to here as a param! 
#>

Login-AzureRmAccount

$storageAccountName = "schoolproject"
$storageAccountKey = "33XbREAAALIt+YEV8CyF+f/69bW5DrCMECnavzFlGNY2bpUzUfuUTHQX8Or/9m7dQD8lNCb1ypQ2Vaq03fnJOg=="
$storageContext = New-AzureStorageContext $storageAccountName -StorageAccountKey $storageAccountKey

New-AzureStorageTable -Name student -Context $storageContext
New-AzureStorageTable -Name teacher -Context $storageContext