using System;
using System.Collections.Generic;
using System.Text;

namespace Filamentverwaltungssystem
{
    // Zentrale Steuerung der Anwendung:
    // - Startmenü (Login / Registrierung / Beenden)
    // - Benutzer- und Admin-Menüs
    // - Ruft passende Services auf (Auth, Inventory, UserManagement)
    public class Application
    {
        // Hält alle persistenten Daten und kümmert sich um JSON-Speicherung
        private readonly DataStore _dataStore;

        // Zuständig für Login / Registrierung
        private readonly AuthService _authService;

        // Zuständig für Filament-, Drucker- und Verbrauchsverwaltung
        private readonly InventoryService _inventoryService;

        // Zuständig für Benutzerverwaltung (nur im Admin-Menü)
        private readonly UserManagement _userManagementService;

        public Application()
        {
            // Daten aus JSON laden (oder leere Strukturen erzeugen)
            _dataStore = new DataStore();
            _dataStore.Load();

            // Services mit den geladenen Daten initialisieren
            _authService = new AuthService(_dataStore.AppData);
            _inventoryService = new InventoryService(_dataStore);
            _userManagementService = new UserManagement(_dataStore);
        }

        // Einstiegspunkt der Konsolen-Anwendung:
        // Hauptmenü für Login / Registrierung / Beenden.
        public void Run()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("Filament Inventar Verwaltung");
                Console.WriteLine("1) Einloggen");
                Console.WriteLine("2) Neu registrieren");
                Console.WriteLine("3) Beenden");
                Console.Write("Auswahl: ");

                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        // Benutzer einloggen
                        var user = _authService.Login();
                        if (user != null)
                        {
                            // Nach erfolgreichem Login passendes Menü anzeigen
                            if (user.Role == UserRole.Admin)
                                ShowAdminMenu(user);
                            else
                                ShowUserMenu(user);
                        }
                        Pause();
                        break;

                    case "2":
                        // Neuen Benutzer registrieren
                        var newUser = _authService.Register();
                        if (newUser != null)
                        {
                            // WICHTIG: Nach Registrierung Daten in JSON speichern
                            _dataStore.SaveAppData();
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

        // Menü für normale Benutzer (ohne Admin-Rechte).
        private void ShowUserMenu(User user)
        {
            bool logout = false;

            while (!logout)
            {
                Console.Clear();
                Console.WriteLine($"Benutzer-Menü ({user.Username})");
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
                        // Verbrauchsdatei einlesen und Filamentmenge anpassen
                        _inventoryService.ProcessUsageFromTxt();
                        Pause();
                        break;
                    case "2":
                        // Alle Filamente sortiert anzeigen
                        _inventoryService.ShowAllFilaments();
                        Pause();
                        break;
                    case "3":
                        // Untermenü Filament-Verwaltung
                        ShowFilamentManagementMenu();
                        break;
                    case "4":
                        // Untermenü Drucker-Verwaltung
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

        // Menü für Administratoren mit zusätzlichen Funktionen:
        // - User-Verwaltung
        // - Statistik über genutzte Filamente und Drucker
        private void ShowAdminMenu(User user)
        {
            bool logout = false;

            while (!logout)
            {
                Console.Clear();
                Console.WriteLine($"Admin-Menü ({user.Username})");
                Console.WriteLine("1) TXT/Gcode hochladen um Verbrauch zu verbuchen");
                Console.WriteLine("2) Alle Filamente anzeigen");
                Console.WriteLine("3) Filament-Verwaltung (hinzufügen/entfernen)");
                Console.WriteLine("4) Drucker-Verwaltung");
                Console.WriteLine("5) User-Verwaltung");
                Console.WriteLine("6) Statistik anzeigen");
                Console.WriteLine("7) Logout");
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
                        ShowUserManagementMenu();
                        break;
                    case "6":
                        // Top 5 Filamente und Drucker anzeigen
                        _inventoryService.ShowStatistics();
                        Pause();
                        break;
                    case "7":
                        logout = true;
                        break;
                    default:
                        Console.WriteLine("Ungültige Eingabe.");
                        Pause();
                        break;
                }
            }
        }

        // Untermenü für Filament-Verwaltung.
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


        // Untermenü für Drucker-Verwaltung.
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

        // Untermenü für Benutzerverwaltung (nur Admin).
        private void ShowUserManagementMenu()
        {
            bool back = false;

            while (!back)
            {
                Console.Clear();
                Console.WriteLine("===== User-Verwaltung =====");
                Console.WriteLine("1) Benutzer anzeigen");
                Console.WriteLine("2) Benutzer anlegen");
                Console.WriteLine("3) Benutzer löschen");
                Console.WriteLine("4) Zurück");
                Console.Write("Auswahl: ");

                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        _userManagementService.ListUsers();
                        Pause();
                        break;
                    case "2":
                        _userManagementService.CreateUserManually();
                        Pause();
                        break;
                    case "3":
                        _userManagementService.DeleteUser();
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

        // Kleine Pause mit Hinweis, damit der Benutzer die Ausgabe lesen kann.
        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Weiter mit beliebiger Taste...");
            Console.ReadKey();
        }
    }
}