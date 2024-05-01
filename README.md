# Benefits of PIM Role Automation Utility
  1. PIM role assignment can be done at Subscription\Resource group level. 
  2. We can do following operation like role assignment\extend\remove\renew.
  3. We can do operations in bulk while in case of manual can do single operation.
  4. We can do role assignment at user level as well as SG level.

# Running Locally
  1. It is Quue based trigger Azure function.
  2. Clone the repo locally.
  3. Copy storage account connection string in AzureWebJobsStorage in local.settings.json.
  4. Assign yourself Role based Access Administrator Role or Owner role at subscription level.
  5. Pass below message in queue for role assignment at resource group level.
     
  [
    {
        "requestType": "Assign",
        "resourceGroupName": "",
        "roleName": "Contributor",
        "scope": "resourcegroup",
        "securityGroupEmail": "",
        "subscriptionId": ""
    }
  ]
    
