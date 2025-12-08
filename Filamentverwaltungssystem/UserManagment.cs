using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Filamentverwaltungssystem
{

    // Bietet Funktionen für die Benutzerverwaltung (nur für Admins gedacht):
    // - Benutzer auflisten
    // - Benutzer manuell anlegen
    // - Benutzer löschen
    public class UserManagement
    {
        private readonly DataStore _dataStore;

        public UserManagement(DataStore dataStore)
        {
            _dataStore = dataStore;
        }


        // Listet alle registrierten Benutzer mit ihrer Rolle auf.
        public void ListUsers()
        {
            if (!_dataStore.AppData.Users.Any())
            {
                Console.WriteLine("Keine Benutzer vorhanden.");
                return;
            }

            Console.WriteLine("Benutzerliste:");
            foreach (var u in _dataStore.AppData.Users)
            {
                Console.WriteLine($"- {u.Username} ({u.Role})");
            }
        }


        // Legt einen neuen Benutzer manuell an (inkl. Wahl der Rolle).
        public void CreateUserManually()
        {
            Console.Write("Benutzername: ");
            string username = Console.ReadLine() ?? string.Empty;

            if (_dataStore.AppData.Users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Benutzername existiert bereits.");
                return;
            }

            Console.Write("Passwort: ");
            string password = AuthService.ReadPassword();

            Console.Write("Rolle (0 = User, 1 = Admin): ");
            string? roleInput = Console.ReadLine();

            UserRole role = UserRole.User;
            if (roleInput == "1")
            {
                role = UserRole.Admin;
            }

            var user = new User
            {
                Username = username,
                Password = password,
                Role = role
            };

            _dataStore.AppData.Users.Add(user);
            _dataStore.SaveAppData();

            Console.WriteLine("Benutzer wurde angelegt.");
        }


        // Löscht einen Benutzer anhand seines Benutzernamens.
        public void DeleteUser()
        {
            ListUsers();
            Console.Write("Benutzernamen des zu löschenden Users eingeben: ");
            string username = Console.ReadLine() ?? string.Empty;

            var user = _dataStore.AppData.Users
                .FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                Console.WriteLine("Benutzer wurde nicht gefunden.");
                return;
            }

            _dataStore.AppData.Users.Remove(user);
            _dataStore.SaveAppData();

            Console.WriteLine("Benutzer wurde gelöscht.");
        }
    }
}