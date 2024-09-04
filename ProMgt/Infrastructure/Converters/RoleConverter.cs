using ProMgt.Client.Models.AuthModels;

namespace ProMgt.Infrastructure.Converters
{
    public static class RoleConverter
    {
        public static List<RoleItem> ConvertToRoleItem(IList<string> userRoles)
        {
            return userRoles.Select(role => new RoleItem
            {
                RoleName = role,
                IsAssigned = true
            }).ToList();
        }
    }
}
