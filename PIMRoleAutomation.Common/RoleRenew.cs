using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PIMRoleAutomation.Common
{
    public class RoleRenew
    {

        [JsonPropertyName("roleName")]
        [Required]
        public string RoleName { get; set; }


        [JsonPropertyName("resourceGroupName")]
        public string ResourceGroupName { get; set; }


        [JsonPropertyName("subscriptionId")]
        [Required]
        public string SubscriptionId { get; set; }


        [JsonPropertyName("tenantId")]
        public string TenantId { get; set; }


        [JsonPropertyName("userPrincipalName")]
        public string UserPrincipalName { get; set; }


        [JsonPropertyName("securityGroupEmail")]
        public string SecurityGroupEmail { get; set; }


        [JsonPropertyName("requestType")]
        public string RequestType { get; set; }


        [JsonPropertyName("scope")]
        [Required]
        public string Scope { get; set; }


        [JsonPropertyName("resourceName")]
        public string ResourceName { get; set; }
    }
}
