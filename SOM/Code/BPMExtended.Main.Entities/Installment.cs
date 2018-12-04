using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class Installment
    {
        public string Id { get; set; }
        public string InvoiceId { get; set; }
        public string Template { get; set; }
        public string Date { get; set; }
        public string Amount { get; set; }
    }
}
