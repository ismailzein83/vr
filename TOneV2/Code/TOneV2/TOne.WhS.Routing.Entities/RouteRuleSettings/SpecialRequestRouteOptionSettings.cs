using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class SpecialRequestRouteOptionSettings : IRouteOptionSettings, ISpecialRequestRouteOptionSettings
    {
        public int SupplierId { get; set; }

        public int NumberOfTries { get; set; }

        public int? Percentage { get; set; }

        public bool ForceOption { get; set; }

        public List<SpecialRequestRouteBackupOptionSettings> Backups { get; set; }
    }

    public class SpecialRequestRouteBackupOptionSettings : IRouteBackupOptionSettings, ISpecialRequestRouteOptionSettings
    {
        public int SupplierId { get; set; }

        public int NumberOfTries { get; set; }

        public bool ForceOption { get; set; }
    }
}