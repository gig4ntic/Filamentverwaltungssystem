using System;
using System.Collections.Generic;
using System.Text;

namespace Filamentverwaltungssystem
{
    //Benutzerrolle Admin oder Basisnutzer
    public enum UserRole
    {
        User = 0,
        Admin = 1
    }

    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
    }
}
