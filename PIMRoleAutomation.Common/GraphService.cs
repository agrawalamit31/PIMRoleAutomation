using Microsoft.Graph;

namespace PIMRoleAutomation.Common
{
    public class GraphService
    {
        private readonly GraphServiceClient graphClient;
        public GraphService(GraphServiceClient graphClient)
        {
            this.graphClient = graphClient;
        }

        public async Task<string?> GetUserObjectId(string userPrincipalName)
        {
            try
            {
                var user = await graphClient.Users[userPrincipalName].Request().GetAsync().ConfigureAwait(false);
                return user?.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred fetching objectId of User: {ex.Message}");
            }

            return null;
        }

        public async Task<string?> GetSecurityGroupObjectId(string sgEmail)
        {
            try
            {
                var result = await graphClient.Groups.Request().Filter("startswith(mail,'" + sgEmail + "')").GetAsync().ConfigureAwait(false); ;
                return result[0].Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred fetching objectId of security group: {ex.Message}");
            }

            return null;
        }
        public async Task<string?> GetRoleDefinitionId(string roleName)
        {
            try
            {

                var roleDefinitions = await graphClient.DirectoryRoles
               .Request()
               .GetAsync();

                var roleDefinition = roleDefinitions.FirstOrDefault(x => x.DisplayName == roleName);
                //var roleDefinitions = await graphClient.RoleManagement.Directory.RoleDefinitions.Request().GetAsync();
                //var roleDefinition = roleDefinitions.FirstOrDefault(r => r.DisplayName.Equals(roleName, StringComparison.OrdinalIgnoreCase));

                return roleDefinition?.Id;// roles?.Value?.FirstOrDefault(r => r.DisplayName == roleName)?.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return null;
        }
    }
}
