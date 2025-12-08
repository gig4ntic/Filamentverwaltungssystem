using System;
using System.Collections.Generic;
using System.Text;

namespace Filamentverwaltungssystem
{

    // Erstellt einen 3D-Drucker.
    public class Printer
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Name} (Id: {Id})";
        }
    }

    // Statistik-Eintrag: Wie oft wurde ein bestimmter Drucker verwendet.
    public class PrinterUsage
    {
        public Guid PrinterId { get; set; }
        public int UsageCount { get; set; }
    }
}