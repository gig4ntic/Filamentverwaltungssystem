using System;
using System.Collections.Generic;
using System.Text;

namespace Filamentverwaltungssystem
{
    //Benutzerrolle Admin oder Basisnutzer
    public enum UserRole
    {
        Basic,
        Admin
    }

    internal class User
    {
        //Benutzername
        public string Username { get; set; }
        //Passwort
        public string Password { get; set; }
        //Rolle Admin oder Basisnutzer
        public UserRole Role { get; set; }
    }
}
