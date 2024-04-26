using Azure.Core;
using Azure.ResourceManager.Authorization.Models;
using System.Xml;

namespace PIMRoleAutomation.Common
{
    public class Helper
    {
        public static RoleManagementScheduleRequestType GetRequestType(string role)
        {
            RoleManagementScheduleRequestType requestType;
            if (role.ToLower() == "extend")
            {
                requestType = RoleManagementScheduleRequestType.AdminExtend;
            }
            else if (role.ToLower() == "remove")
            {
                requestType = RoleManagementScheduleRequestType.AdminRemove;
            }
            else if (role.ToLower() == "renew")
            {
                requestType = RoleManagementScheduleRequestType.AdminRenew;
            }
            else
            {
                requestType = RoleManagementScheduleRequestType.AdminAssign;
            }

            return requestType;
        }

        public static ResourceIdentifier GetScopeId(RoleRenew roleRenew)
        {
            if (roleRenew.Scope.ToLower() == "resource")
            {
                var resourceScope = $"subscriptions/{roleRenew.SubscriptionId}/resourceGroups/{roleRenew.ResourceGroupName}/providers/Microsoft.DigitalTwins/digitalTwinsInstances/{roleRenew.ResourceName}";
                return new ResourceIdentifier(string.Format("/{0}", resourceScope));
            }
            else if (roleRenew.Scope.ToLower() == "resourcegroup")
            {
                var rgScope = $"/subscriptions/{roleRenew.SubscriptionId}/resourceGroups/{roleRenew.ResourceGroupName}";
                return new ResourceIdentifier(string.Format("/{0}", rgScope));
            }
            else if (roleRenew.Scope.ToLower() == "subscription")
            {
                var subscriptionScope = $"subscriptions/{roleRenew.SubscriptionId}";
                return new ResourceIdentifier(string.Format("/{0}", subscriptionScope));
            }

            return null;
        }

        public static TimeSpan GetDuration(string roleDefinitionId)
        {
            TimeSpan duration;
            if (roleDefinitionId == "acdd72a7-3385-48ef-bd42-f606fba81ae7") // reader Role
            {
                duration = XmlConvert.ToTimeSpan("P365D");
            }
            else
            {
                duration = XmlConvert.ToTimeSpan("P180D");
            }

            return duration;
        }

    }
}
