namespace ProMgt.Client.Models.AuthModels
{
    public class RoleInputModel
    {
        public string Id { get; set; } = string.Empty;
        public bool IsUserRole { get; set; }
        public List<RoleItem> Roles { get; set; } = new List<RoleItem>();
    }

    public class RoleItem
    {
        public string Id { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsAssigned { get; set; }
    }
}
