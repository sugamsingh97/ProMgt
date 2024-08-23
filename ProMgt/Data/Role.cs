using Microsoft.AspNetCore.Identity;

namespace ProMgt.Data
{
    public class Role : IdentityRole
    {
        public Role() : base()
        {
            
        }
        public Role(string roleName) : base(roleName)
        {
            
        }
    }
}
