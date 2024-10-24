using ProMgt.Client.Models.AuthModels;

namespace ProMgt.Client;

/// <summary>
/// Profile info class
/// </summary>
public class UserInfo
{
    public required string UserId { get; set; }
    public required string Email { get; set; }
    public List<RoleItem> Roles { get; set; } = new List<RoleItem>();
}
//TODO: Add properties to this class and update the server and client AuthenticationStateProviders
// to expose more information about the authenticated user to the client.
