using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RouteOptionSettings
    {
        public int SupplierId { get; set; }

        public Decimal? Percentage { get; set; }
    }

    public class SpecialRequestOptionSettings
    {
        public int SupplierId { get; set; }

        public int Position { get; set; }

        public bool ForceOption { get; set; }

        public int NumberOfTries { get; set; }

        public Decimal? Percentage { get; set; }
    }
}