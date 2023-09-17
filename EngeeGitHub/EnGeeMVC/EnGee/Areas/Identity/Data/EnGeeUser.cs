using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EnGee.Areas.Identity.Data;

// Add profile data for application users by adding properties to the EnGeeUser class
public class EnGeeUser : IdentityUser
{
    //----------------------0917 Min新增--------------------------//
    public string PasswordHash { get; set; }
    //----------------------------------------------------------//
    public int MemberId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Fullname { get; set; } = null!;

    public string Nickname { get; set; } = null!;


    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public DateTime RegistrationDate { get; set; }

    public DateTime Birth { get; set; }

    public int Access { get; set; }

    public double Point { get; set; }

    public string? PhotoPath { get; set; }

    public string? Introduction { get; set; }

    public string? CharityProof { get; set; }


}

