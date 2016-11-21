using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.NumberingPlan.Entities;

namespace TOne.WhS.CodePreparation.Entities
{
    public class ChangesDetail
    {
        public IEnumerable<ZoneChangesDetail> ZoneChanges { get; set; }
        //public IEnumerable<CodeChangesDetail> CodeChanges { get; set; }
    }

    //public class CodeChangesDetail
    //{
    //    public ChangedCode Entity { get; set; }
    //}

    public class ZoneChangesDetail
    {
        public NewZone Entity { get; set; }
        public string ZoneName { get; set; }
    }
}
