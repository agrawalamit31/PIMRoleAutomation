using Azure.Identity;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Newtonsoft.Json;
using PIMRoleAutomation.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PIMRoleAutomation
{
    public class PIMRoleRenew
    {
        [FunctionName("PIMRoleRenew")]
        public async Task Run([QueueTrigger("testqueue")] string myQueueItem, ILogger log)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(myQueueItem))
                {
                    log.LogError("Queue message is empty");
                    return;
                }

                var credential = new ChainedTokenCredential(new DefaultAzureCredential());
                var scope = new[] { "https://graph.microsoft.com/.default" };

                // Create a new instance of GraphServiceClient with the authentication provider
                var graphService = new GraphService(new GraphServiceClient(credential, scope));
                var pimRoleAssignment = new PIMEligibleRoleAssignment(graphService);
                var rolesToRenew = JsonConvert.DeserializeObject<List<RoleRenew>>(myQueueItem);
                foreach (var roleRenew in rolesToRenew)
                {
                    var isAssigmenetCompleted = await pimRoleAssignment.IsEligibleRoleAssignmentCompleted(roleRenew).ConfigureAwait(false);

                    if (isAssigmenetCompleted)
                    {
                        log.LogInformation($"Role assignment completed for Principal : {roleRenew.UserPrincipalName}, Role Name: {roleRenew.RoleName}, ResourceGroup : {roleRenew.ResourceGroupName} ");
                    }
                    else
                    {
                        log.LogInformation($"Role assignment failed for Principal : {roleRenew.UserPrincipalName}, Role Name: {roleRenew.RoleName}, ResourceGroup : {roleRenew.ResourceGroupName} ");
                    }
                }
            }
            catch(Exception ex)
            {
                log.LogError($"Error occured while assigning role" , ex);
            }
        }
    }
}
