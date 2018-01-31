using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class SpecialRequestRouteOptionSettings
    {
        public int SupplierId { get; set; }

        public int Position { get; set; }

        public bool ForceOption { get; set; }

        public int NumberOfTries { get; set; }

        public int? Percentage { get; set; }

        public List<SpecialRequestRouteBackupOptionSettings> Backups { get; set; }
    }

    public class SpecialRequestRouteBackupOptionSettings
    {
        public int SupplierId { get; set; }

        public int Position { get; set; }

        public bool ForceOption { get; set; }
    }
}
