using System;
using System.Collections.Generic;
using System.Text;

namespace Filamentverwaltungssystem
{
    public class ApplicationController
    {
        private readonly DataStore _dataStore;
        private readonly AuthService _authService;

        public ApplicationController()
        {
            _dataStore = new DataStore();
            _dataStore.Load();

            _authService = new AuthService(_dataStore.AppData);
        }

        public void Run()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== Filament Inventar Verwaltung ===");
                Console.WriteLine("1) Einloggen");
                Console.WriteLine("2) Neu registrieren");
                Console.WriteLine("3) Beenden");
                Console.Write("Auswahl: ");

                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        var user = _authService.Login();
                        if (user != null)
                        {
                            if (user.Role == UserRole.Admin)
                                ShowAdminMenuPlaceholder(user);
                            else
                                ShowUserMenuPlaceholder(user);
                        }
                        Pause();
                        break;

                    case "2":
                        var newUser = _authService.Register();
                        if (newUser != null)
                        {
                            Console.WriteLine("Sie können sich nun einloggen.");
                        }
                        Pause();
                        break;

                    case "3":
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Ungültige Eingabe.");
                        Pause();
                        break;
                }
            }
        }

        private void ShowUserMenuPlaceholder(User user)
        {
            Console.WriteLine();
            Console.WriteLine("Benutzer-Menü");
            Console.WriteLine($"Eingeloggt als: {user.Username} ");
            Console.WriteLine("Morgen hier weiter");
            Console.WriteLine("!!!!!!!!!!!!!!!!");
        }

        private void ShowAdminMenuPlaceholder(User user)
        {
            Console.WriteLine();
            Console.WriteLine("Admin-Menü");
            Console.WriteLine($"Eingeloggt als: {user.Username}");
            Console.WriteLine("Morgen hier weiter");
            Console.WriteLine("!!!!!!!!!!!!!!!!");
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Weiter mit beliebiger Taste:");
            Console.ReadKey();
        }
    }
}
