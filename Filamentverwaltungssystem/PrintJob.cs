using System;
using System.Collections.Generic;
using System.Text;

namespace Filamentverwaltungssystem
{
    // Extrahiert die Daten eines Druckauftrags,
    // wie sie aus der TXT/GCode-Hilfsdatei gelesen werden.
    public class PrintJob
    {
        public string FilamentType { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public double Diameter { get; set; }
        public double AmountGrams { get; set; }
        public string PrinterName { get; set; } = string.Empty;
    }
}