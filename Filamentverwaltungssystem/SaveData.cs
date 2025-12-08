using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.IO;

namespace Filamentverwaltungssystem
{
    // Container-Klasse für alle speicherbaren Daten:
    // Benutzer, Filamente, Drucker.
    // Wird als data.json gespeichert.
    public class AppData
    {
        public List<User> Users { get; set; } = new();
        public List<Filament> Filaments { get; set; } = new();
        public List<Printer> Printers { get; set; } = new();
    }

    // Container für die Statistik-Daten:
    // - Filamentnutzung
    // - Druckernutzung
    public class StatisticsData
    {
        // Wie oft wurde welches Filament verwendet?
        public List<Filament.FilamentUsage> FilamentUsage { get; set; } = new();

        // Wie oft wurde welcher Drucker verwendet?
        public List<PrinterUsage> PrinterUsage { get; set; } = new();
    }

    // Kapselt das Laden und Speichern der Daten in JSON-Dateien.
    public class DataStore
    {
        private const string AppDataFile = "data.json";
        private const string StatsDataFile = "stats.json";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        public AppData AppData { get; private set; } = new();
        public StatisticsData Statistics { get; private set; } = new();

        // Lädt AppData und Statistics aus JSON-Dateien.
        // Erstellt falls nötig neue leere Objekte.
        // Erzeugt außerdem den Default-Admin.
        public void Load()
        {
            AppData = LoadFromFile<AppData>(AppDataFile) ?? new AppData();
            Statistics = LoadFromFile<StatisticsData>(StatsDataFile) ?? new StatisticsData();

            DefaultAdmin();
        }

        // Generische Hilfsmethode zum Laden eines JSON-Files in ein Objekt.
        private T? LoadFromFile<T>(string fileName) where T : class
        {
            try
            {
                if (!File.Exists(fileName))
                    return null;

                string json = File.ReadAllText(fileName);
                if (string.IsNullOrWhiteSpace(json))
                    return null;

                return JsonSerializer.Deserialize<T>(json, JsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden von {fileName}: {ex.Message}");
                return null;
            }
        }

        // Speichert AppData (Benutzer, Filamente, Drucker) in data.json.
        public void SaveAppData()
        {
            SaveToFile(AppDataFile, AppData);
        }

        // Speichert die Statistikdaten in stats.json.
        public void SaveStatistics()
        {
            SaveToFile(StatsDataFile, Statistics);
        }

        // Generische Hilfsmethode zum Speichern eines Objekts als JSON.
        private void SaveToFile<T>(string fileName, T data)
        {
            try
            {
                string json = JsonSerializer.Serialize(data, JsonOptions);
                File.WriteAllText(fileName, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Speichern von {fileName}: {ex.Message}");
            }
        }

        // Stellt sicher, dass es mindestens einen Admin-User gibt.
        // Falls nicht, wird 'admin'/'admin' angelegt.
        private void DefaultAdmin()
        {
            // Admin Hardcoded
            bool adminExists = AppData.Users.Exists(u => u.Role == UserRole.Admin);
            if (!adminExists)
            {
                AppData.Users.Add(new User
                {
                    Username = "admin",
                    Password = "admin",
                    Role = UserRole.Admin
                });
                SaveAppData();
            }
        }
    }
}