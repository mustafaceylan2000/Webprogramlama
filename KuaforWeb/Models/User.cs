using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace KuaforWeb.Models;

public class User : IdentityUser
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(50, ErrorMessage = "Name cannot be longer than 50 characters")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Surname is required")]
    [MaxLength(50, ErrorMessage = "Surname cannot be longer than 50 characters")]
    public string Surname { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

