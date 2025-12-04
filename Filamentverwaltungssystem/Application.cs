using System;
using System.Collections.Generic;
using System.Text;

namespace Filamentverwaltungssystem
{
    public class Application
    {
        private readonly DataStore _dataStore;
        private readonly AuthService _authService;
        private readonly InventoryService _inventoryService;

        public Application()
        {
            _dataStore = new DataStore();
            _dataStore.Load();

            _authService = new AuthService(_dataStore.AppData);
            _inventoryService = new InventoryService(_dataStore);
        }

        public void Run()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("Filament Inventar");
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
                                ShowAdminMenu(user);
                            else
                                ShowUserMenu(user);
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

        private void ShowUserMenu(User user)
        {
            bool logout = false;

            while (!logout)
            {
                Console.Clear();
                Console.WriteLine($"Menü für ({user.Username})");
                Console.WriteLine("1) TXT/Gcode hochladen um Verbrauch zu verbuchen");
                Console.WriteLine("2) Alle Filamente anzeigen");
                Console.WriteLine("3) Filament-Verwaltung (hinzufügen/entfernen)");
                Console.WriteLine("4) Drucker-Verwaltung");
                Console.WriteLine("5) Logout");
                Console.Write("Auswahl: ");

                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        _inventoryService.ProcessUsageFromTxt();
                        Pause();
                        break;
                    case "2":
                        _inventoryService.ShowAllFilaments();
                        Pause();
                        break;
                    case "3":
                        ShowFilamentManagementMenu();
                        break;
                    case "4":
                        ShowPrinterManagementMenu();
                        break;
                    case "5":
                        logout = true;
                        break;
                    default:
                        Console.WriteLine("Ungültige Eingabe.");
                        Pause();
                        break;
                }
            }
        }

        private void ShowAdminMenu(User user)
        {
            bool logout = false;

            while (!logout)
            {
                Console.Clear();
                Console.WriteLine($"Admin-Menü für ({user.Username})");
                Console.WriteLine("1) TXT/Gcode hochladen um Verbrauch zu verbuchen");
                Console.WriteLine("2) Alle Filamente anzeigen");
                Console.WriteLine("3) Filament-Verwaltung (hinzufügen/entfernen)");
                Console.WriteLine("4) Drucker-Verwaltung");
                Console.WriteLine("5) Logout");
                Console.Write("Auswahl: ");

                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        _inventoryService.ProcessUsageFromTxt();
                        Pause();
                        break;
                    case "2":
                        _inventoryService.ShowAllFilaments();
                        Pause();
                        break;
                    case "3":
                        ShowFilamentManagementMenu();
                        break;
                    case "4":
                        ShowPrinterManagementMenu();
                        break;
                    case "5":
                        logout = true;
                        break;
                    default:
                        Console.WriteLine("Ungültige Eingabe.");
                        Pause();
                        break;
                }
            }
        }

        private void ShowFilamentManagementMenu()
        {
            bool back = false;

            while (!back)
            {
                Console.Clear();
                Console.WriteLine("Filament-Verwaltung");
                Console.WriteLine("1) Neues Filament hinzufügen");
                Console.WriteLine("2) Filament entfernen");
                Console.WriteLine("3) Zurück");
                Console.Write("Auswahl: ");

                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        _inventoryService.AddFilament();
                        Pause();
                        break;
                    case "2":
                        _inventoryService.RemoveFilament();
                        Pause();
                        break;
                    case "3":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Ungültige Eingabe.");
                        Pause();
                        break;
                }
            }
        }

        private void ShowPrinterManagementMenu()
        {
            bool back = false;

            while (!back)
            {
                Console.Clear();
                Console.WriteLine("Drucker-Verwaltung");
                Console.WriteLine("1) Drucker anzeigen");
                Console.WriteLine("2) Neuen Drucker anlegen");
                Console.WriteLine("3) Drucker entfernen");
                Console.WriteLine("4) Zurück");
                Console.Write("Auswahl: ");

                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        _inventoryService.ShowPrinters();
                        Pause();
                        break;
                    case "2":
                        _inventoryService.AddPrinter();
                        Pause();
                        break;
                    case "3":
                        _inventoryService.RemovePrinter();
                        Pause();
                        break;
                    case "4":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Ungültige Eingabe.");
                        Pause();
                        break;
                }
            }
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Weiter mit beliebiger Taste...");
            Console.ReadKey();
        }
    }
}