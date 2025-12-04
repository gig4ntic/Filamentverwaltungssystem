using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Filamentverwaltungssystem
{
    // Container zum speichern für alle Daten
    public class AppData
    {
        public List<User> Users { get; set; } = new();
        public List<Filament> Filaments { get; set; } = new();
        public List<Printer> Printers { get; set; } = new();
    }

    // Statistikdaten
    public class StatisticsData
    {
        // Filamentnutzung
        public List<Filament.FilamentUsage> FilamentUsage { get; set; } = new();

        // Druckernutzung
        public List<PrinterUsage> PrinterUsage { get; set; } = new();
    }

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

        public void Load()
        {
            AppData = LoadFromFile<AppData>(AppDataFile) ?? new AppData();
            Statistics = LoadFromFile<StatisticsData>(StatsDataFile) ?? new StatisticsData();

            EnsureDefaultAdmin();
        }

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

        public void SaveAppData()
        {
            SaveToFile(AppDataFile, AppData);
        }

        public void SaveStatistics()
        {
            SaveToFile(StatsDataFile, Statistics);
        }

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

        private void EnsureDefaultAdmin()
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
