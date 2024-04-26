using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Authorization;
using Azure.ResourceManager.Authorization.Models;

namespace PIMRoleAutomation.Common
{
    public class PIMEligibleRoleAssignment
    {
        private readonly GraphService graphService;
        public PIMEligibleRoleAssignment(GraphService graphService)
        {
            this.graphService = graphService;
        }

        public async Task<bool> IsEligibleRoleAssignmentCompleted(RoleRenew roleRenew)
        {
            bool IsAssignmentCompleted;
            try
            {
                TokenCredential cred = new DefaultAzureCredential();              
                DateTime currentDateTime = DateTime.UtcNow;
                string utcDateTime = currentDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
                ArmClient client = new ArmClient(cred);
                                
                string principalId = await GetPrincipalId(roleRenew).ConfigureAwait(false);
                var scopeId = Helper.GetScopeId(roleRenew);
                var roleDefinitionId = await GetRoleDefinitionIdAsync(roleRenew.SubscriptionId, 
                    roleRenew.RoleName, client);   

                if (string.IsNullOrWhiteSpace(roleDefinitionId) || 
                    string.IsNullOrWhiteSpace(principalId) || scopeId == null)
                {
                    IsAssignmentCompleted = false;
                    return IsAssignmentCompleted;
                }              
                
                var collection = client.GetRoleEligibilityScheduleRequests(scopeId);
                string roleEligibilityScheduleRequestName = Guid.NewGuid().ToString();
                var data = new RoleEligibilityScheduleRequestData()
                {
                    RoleDefinitionId = new ResourceIdentifier($"/subscriptions/{roleRenew.SubscriptionId}/providers/Microsoft.Authorization/roleDefinitions/{roleDefinitionId}"),
                    PrincipalId = Guid.Parse(principalId),
                    RequestType = Helper.GetRequestType(roleRenew.RequestType),
                    StartOn = DateTimeOffset.Parse(utcDateTime),
                    ExpirationType = RoleManagementScheduleExpirationType.AfterDuration,
                    EndOn = null,
                    Duration = Helper.GetDuration(roleDefinitionId),
                };

                ArmOperation<RoleEligibilityScheduleRequestResource> lro = await collection.CreateOrUpdateAsync(WaitUntil.Completed, roleEligibilityScheduleRequestName, data).ConfigureAwait(false);
                RoleEligibilityScheduleRequestResource result = lro.Value;
                IsAssignmentCompleted = result.Data != null;
            }
            catch (Exception ex)
            {
                IsAssignmentCompleted = false;
            }
            return IsAssignmentCompleted;
        }

        private async Task<string> GetPrincipalId(RoleRenew roleRenew)
        {
            string principalId = string.Empty;
            if (!string.IsNullOrWhiteSpace(roleRenew.UserPrincipalName))
            {
                principalId = await graphService.GetUserObjectId(roleRenew.UserPrincipalName).ConfigureAwait(false);
            }
            else if (!string.IsNullOrWhiteSpace(roleRenew.SecurityGroupEmail))
            {
                principalId = await graphService.GetSecurityGroupObjectId(roleRenew.SecurityGroupEmail).ConfigureAwait(false);
            }
            
            return principalId;
        }
       
        private async Task<string > GetRoleDefinitionIdAsync(string subscriptionId,string roleName, ArmClient client)
        {
            string roleAssignmentIdToAssign = string.Empty;
            string scope = $"/subscriptions/{subscriptionId}";
            
            var roleDefinitionsOperations = client
                                            .GetAuthorizationRoleDefinitions(new ResourceIdentifier(scope))
                                            .GetAllAsync()
                                            .GetAsyncEnumerator();           

            while (await roleDefinitionsOperations.MoveNextAsync())
            {
                if (roleDefinitionsOperations.Current.Data.RoleName.Equals(roleName))
                {
                    roleAssignmentIdToAssign = roleDefinitionsOperations.Current.Data.Name;
                    break;
                }
            }

            return roleAssignmentIdToAssign;
        }
    }
}
