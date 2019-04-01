using System;

namespace BPMExtended.Main.SOMAPI
{
    public class Installment
    {
        public string Id { get; set; }
        public DateTime DueDate { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
    }
}
