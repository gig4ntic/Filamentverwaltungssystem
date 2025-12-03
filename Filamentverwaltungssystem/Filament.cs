using System;
using System.Collections.Generic;
using System.Text;

namespace Filamentverwaltungssystem
{
    public class Filament
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Type { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public double Diameter { get; set; }
        public double RemainingGrams { get; set; }

        public override string ToString()
        {
            return $"{Type} | {Color} | Ø {Diameter}mm | {RemainingGrams}g (Id: {Id})";
        }

        public class FilamentUsageEntry
        {
            public Guid FilamentId { get; set; }
            public int UsageCount { get; set; }
        }
    }
}