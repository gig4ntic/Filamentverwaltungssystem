using System;
using System.Collections.Generic;
using System.Text;

namespace Filamentverwaltungssystem
{
    public class Printer
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Name} (Id: {Id})";
        }
    }

    public class PrinterUsage
    {
        public Guid PrinterId { get; set; }
        public int UsageCount { get; set; }
    }
}