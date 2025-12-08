using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Filamentverwaltungssystem
{

    // Verwaltet Login und Registrierung von Benutzern.
    // Nutzt dabei die in AppData gespeicherten Benutzer.
    public class AuthService
    {
        private readonly AppData _appData;

        public AuthService(AppData appData)
        {
            _appData = appData;
        }

        // Führt einen Login-Versuch durch.
        // Fragt Benutzername und Passwort ab und liefert den passenden User oder null.
        public User? Login()
        {
            Console.Write("Benutzername: ");
            string username = Console.ReadLine() ?? string.Empty;

            Console.Write("Passwort: ");
            string password = ReadPassword();

            var user = _appData.Users
                .FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                                     && u.Password == password);

            if (user == null)
            {
                Console.WriteLine("Login fehlgeschlagen.");
            }
            else
            {
                Console.WriteLine($"Erfolgreich eingeloggt als {user.Username} ({user.Role}).");
            }

            return user;
        }

        // Registriert einen neuen Benutzer mit Rolle "User".
        public User? Register()
        {
            Console.Write("Neuer Benutzername: ");
            string username = Console.ReadLine() ?? string.Empty;

            // Prüfen, ob Benutzername schon existiert
            if (_appData.Users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Benutzername existiert bereits.");
                return null;
            }

            Console.Write("Passwort: ");
            string password = ReadPassword();

            var user = new User
            {
                Username = username,
                Password = password,
                Role = UserRole.User
            };

            _appData.Users.Add(user);
            Console.WriteLine("Benutzer erfolgreich registriert.");

            return user;
        }

        // Liest ein Passwort verdeckt von der Konsole (mit *).
        public static string ReadPassword()
        {
            string password = string.Empty;
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    // Letztes Zeichen entfernen
                    password = password[0..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    // Zeichen hinzufügen und Stern ausgeben
                    password += key.KeyChar;
                    Console.Write("*");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }
    }
} 