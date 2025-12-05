using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Filamentverwaltungssystem
{
    public class InventoryService
    {
        private readonly DataStore _dataStore;

        public InventoryService(DataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public void ProcessUsageFromTxt()
        {
            Console.Write("Pfad zur TXT-Datei eingeben: ");
            string? path = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                Console.WriteLine("Datei wurde nicht gefunden.");
                return;
            }

            var job = ParsePrintJobFromFile(path);
            if (job == null)
            {
                Console.WriteLine("Die Datei konnte nicht korrekt gelesen werden.");
                return;
            }

            var filament = _dataStore.AppData.Filaments
                .FirstOrDefault(f =>
                    f.Type.Equals(job.FilamentType, StringComparison.OrdinalIgnoreCase) &&
                    f.Color.Equals(job.Color, StringComparison.OrdinalIgnoreCase) &&
                    Math.Abs(f.Diameter - job.Diameter) < 0.001);

            if (filament == null)
            {
                Console.WriteLine("Passendes Filament wurde nicht gefunden.");
                return;
            }

            var printer = _dataStore.AppData.Printers
                .FirstOrDefault(p => p.Name.Equals(job.PrinterName, StringComparison.OrdinalIgnoreCase));

            if (printer == null)
            {
                Console.WriteLine("Passender Drucker wurde nicht gefunden.");
                return;
            }

            Console.WriteLine($"Gefundenes Filament: {filament}");
            Console.WriteLine($"Gefundener Drucker: {printer.Name}");
            Console.WriteLine($"Verbrauchtes Filament laut Datei: {job.AmountGrams} g");

            if (filament.RemainingGrams < job.AmountGrams)
            {
                Console.WriteLine("WARNUNG: Nicht genug Filament vorhanden! Vorgang wird nicht durchgeführt. Bitte neues Filament bestellen!");
                return;
            }

            filament.RemainingGrams -= job.AmountGrams;
            Console.WriteLine($"Neue Restmenge des Filaments: {filament.RemainingGrams} g");

            _dataStore.SaveAppData();
        }

        private PrintJob? ParsePrintJobFromFile(string path)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var line in File.ReadAllLines(path))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split('=', 2);
                if (parts.Length != 2)
                    continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim();
                dict[key.ToLowerInvariant()] = value;
            }

            if (!dict.TryGetValue("filamenttype", out string type) &&
                !dict.TryGetValue("type", out type))
            {
                Console.WriteLine("Filamentart fehlt in der Datei.");
                return null;
            }

            if (!dict.TryGetValue("color", out string color))
            {
                Console.WriteLine("Farbe des Filaments fehlt in der Datei.");
                return null;
            }

            if (!dict.TryGetValue("diameter", out string diameterStr) ||
                !double.TryParse(diameterStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double diameter))
            {
                Console.WriteLine("Durchmesser ist ungültig oder fehlt.");
                return null;
            }

            if (!dict.TryGetValue("amountgrams", out string gramsStr) ||
                !double.TryParse(gramsStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double grams))
            {
                Console.WriteLine("Keine Filamentmenge in der Datei gefunden.");
                return null;
            }

            if (!dict.TryGetValue("printer", out string printerName))
            {
                Console.WriteLine("Kein Drucker geunden in der Datei.");
                return null;
            }

            return new PrintJob
            {
                FilamentType = type,
                Color = color,
                Diameter = diameter,
                AmountGrams = grams,
                PrinterName = printerName
            };
        }


        // Filament-Verwaltung


        public void ShowAllFilaments()
        {
            var filaments = _dataStore.AppData.Filaments
                .OrderBy(f => f.Type)
                .ThenBy(f => f.Color)
                .ToList();

            if (!filaments.Any())
            {
                Console.WriteLine("Keine Filamente vorhanden.");
                return;
            }

            Console.WriteLine("Filament-Liste:");
            foreach (var f in filaments)
            {
                Console.WriteLine($"- {f}");
            }
        }

        public void AddFilament()
        {
            Console.Write("Filamentart (PLA, PETG, etc.): ");
            string type = Console.ReadLine() ?? string.Empty;

            Console.Write("Farbe (rot, blau, etc.): ");
            string color = Console.ReadLine() ?? string.Empty;

            Console.Write("Durchmesser (1.75mm oder 3mm): ");
            if (!double.TryParse(Console.ReadLine(), NumberStyles.Float, CultureInfo.InvariantCulture, out double diameter))
            {
                Console.WriteLine("Ungültiger Durchmesser.");
                return;
            }

            Console.Write("Menge in Gramm: ");
            if (!double.TryParse(Console.ReadLine(), NumberStyles.Float, CultureInfo.InvariantCulture, out double grams))
            {
                Console.WriteLine("Ungültige Gramm-Angabe.");
                return;
            }

            var filament = new Filament
            {
                Type = type,
                Color = color,
                Diameter = diameter,
                RemainingGrams = grams
            };

            _dataStore.AppData.Filaments.Add(filament);
            _dataStore.SaveAppData();

            Console.WriteLine("Filament wurde hinzugefügt.");
        }

        public void RemoveFilament()
        {
            ShowAllFilaments();
            Console.Write("Geben Sie die Id des zu löschenden Filaments ein: ");
            string? idStr = Console.ReadLine();

            if (!Guid.TryParse(idStr, out Guid id))
            {
                Console.WriteLine("Ungültige Id.");
                return;
            }

            var filament = _dataStore.AppData.Filaments.FirstOrDefault(f => f.Id == id);
            if (filament == null)
            {
                Console.WriteLine("Filament wurde nicht gefunden.");
                return;
            }

            _dataStore.AppData.Filaments.Remove(filament);
            _dataStore.SaveAppData();

            Console.WriteLine("Filament wurde entfernt.");
        }

        private void IncreaseFilamentUsage(Guid filamentId)
        {
            var entry = _dataStore.Statistics.FilamentUsage
                .FirstOrDefault(e => e.FilamentId == filamentId);

            if (entry == null)
            {
                entry = new Filament.FilamentUsage
                {
                    FilamentId = filamentId,
                    UsageCount = 0
                };
                _dataStore.Statistics.FilamentUsage.Add(entry);
            }

            entry.UsageCount++;
        }


        // Drucker-Verwaltung


        public void ShowPrinters()
        {
            var printers = _dataStore.AppData.Printers.ToList();

            if (!printers.Any())
            {
                Console.WriteLine("Keine Drucker vorhanden.");
                return;
            }

            Console.WriteLine("Drucker-Liste:");
            foreach (var p in printers)
            {
                Console.WriteLine($"- {p}");
            }
        }

        public void AddPrinter()
        {
            Console.Write("Name des neuen Druckers: ");
            string name = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Name darf nicht leer sein.");
                return;
            }

            var printer = new Printer { Name = name };
            _dataStore.AppData.Printers.Add(printer);
            _dataStore.SaveAppData();

            Console.WriteLine("Drucker wurde hinzugefügt.");
        }

        public void RemovePrinter()
        {
            ShowPrinters();
            Console.Write("Geben Sie die Id des zu löschenden Druckers ein: ");
            string? idStr = Console.ReadLine();

            if (!Guid.TryParse(idStr, out Guid id))
            {
                Console.WriteLine("Ungültige Id.");
                return;
            }

            var printer = _dataStore.AppData.Printers.FirstOrDefault(p => p.Id == id);
            if (printer == null)
            {
                Console.WriteLine("Drucker wurde nicht gefunden.");
                return;
            }

            _dataStore.AppData.Printers.Remove(printer);
            _dataStore.SaveAppData();

            Console.WriteLine("Drucker wurde entfernt.");
        }

        private void IncreasePrinterUsage(Guid printerId)
        {
            var entry = _dataStore.Statistics.PrinterUsage
                .FirstOrDefault(e => e.PrinterId == printerId);

            if (entry == null)
            {
                entry = new PrinterUsage
                {
                    PrinterId = printerId,
                    UsageCount = 0
                };
                _dataStore.Statistics.PrinterUsage.Add(entry);
            }

            entry.UsageCount++;
        }

        //Statistik anzeigen

        public void ShowStatistics()
        {
            Console.WriteLine("Top 5 Filamente (nach Häufigkeit der Nutzung):");

            var topFilaments = _dataStore.Statistics.FilamentUsage
                .OrderByDescending(f => f.UsageCount)
                .Take(5)
                .ToList();

            if (!topFilaments.Any())
            {
                Console.WriteLine("- Keine Daten vorhanden.");
            }
            else
            {
                foreach (var entry in topFilaments)
                {
                    var filament = _dataStore.AppData.Filaments.FirstOrDefault(f => f.Id == entry.FilamentId);
                    string name = filament != null
                        ? $"{filament.Type} | {filament.Color} | Ø {filament.Diameter}mm"
                        : "Unbekanntes Filament";

                    Console.WriteLine($"- {name}: {entry.UsageCount} Verwendungen");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Top 5 Drucker (nach Häufigkeit der Nutzung):");

            var topPrinters = _dataStore.Statistics.PrinterUsage
                .OrderByDescending(p => p.UsageCount)
                .Take(5)
                .ToList();

            if (!topPrinters.Any())
            {
                Console.WriteLine("- Keine Daten vorhanden.");
            }
            else
            {
                foreach (var entry in topPrinters)
                {
                    var printer = _dataStore.AppData.Printers.FirstOrDefault(p => p.Id == entry.PrinterId);
                    string name = printer != null ? printer.Name : "Unbekannter Drucker";

                    Console.WriteLine($"- {name}: {entry.UsageCount} Verwendungen");
                }
            }
        }
    }
}