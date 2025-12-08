using System;
using System.Collections.Generic;
using System.Text;

namespace Filamentverwaltungssystem
{

    // Erstellt eine einzelne Filament-Rolle im Lager.
    public class Filament
    {
        public Guid Id { get; set; } = Guid.NewGuid();       // Eindeutige automatisch generierte ID
        public string Type { get; set; } = string.Empty;     // z.B. PLA, PETG, ABS
        public string Color { get; set; } = string.Empty;    // z.B. rot, blau, grün
        public double Diameter { get; set; }                 // entweder 1.75 oder 3.0 mm
        public double RemainingGrams { get; set; }           // Restmenge in Gramm

        public override string ToString()
        {
            return $"{Type} | {Color} | Ø {Diameter}mm | {RemainingGrams}g (Id: {Id})";
        }

        // Statistik-Eintrag: Wie oft wurde eine bestimmte Filamentrolle verwendet.
        public class FilamentUsage
        {
            public Guid FilamentId { get; set; }
            public int UsageCount { get; set; }
        }
    }
}