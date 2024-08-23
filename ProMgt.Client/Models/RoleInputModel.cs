namespace ProMgt.Client.Models
{
    public class RoleInputModel
    {
        public string Id { get; set; }
        public bool IsUserRole { get; set; }
        public List<RoleItem> Roles { get; set; }
    }

    public class RoleItem
    {
        public string Id { get; set; }
        public string NormalizedName { get; set; }
        public string RoleName { get; set; }
        public bool IsAssigned { get; set; }
    }
}
