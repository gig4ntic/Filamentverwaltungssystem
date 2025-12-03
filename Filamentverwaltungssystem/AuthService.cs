using System;
using System.Collections.Generic;
using System.Text;

namespace Filamentverwaltungssystem
{
    public class AuthService
    {
        private readonly AppData _appData;

        public AuthService(AppData appData)
        {
            _appData = appData;
        }

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

        public User? Register()
        {
            Console.Write("Neuer Benutzername: ");
            string username = Console.ReadLine() ?? string.Empty;

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

        public static string ReadPassword()
        {
            string password = string.Empty;
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[0..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }
    }
}
