using Microsoft.AspNetCore.Identity;

namespace SmartWarehouse.Domain.Entities;

/// <summary>
/// Application role extending ASP.NET Core Identity.
/// We use IdentityRole which provides Id, Name, NormalizedName, ConcurrencyStamp.
/// Additional domain properties can be added here if needed.
/// </summary>
public class Role : IdentityRole
{
    public string? Description { get; set; }

    public Role() : base() { }

    public Role(string roleName) : base(roleName) { }
}
