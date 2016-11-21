using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public class Changes
    {
        public Changes()
        {
            DeletedCodes = new List<DeletedCode>();
            NewCodes = new List<NewCode>();
            NewZones = new List<NewZone>();
            RenamedZones = new List<RenamedZone>();
            DeletedZones = new List<DeletedZone>();
        }
        public List<DeletedCode> DeletedCodes { get; set; }
        public List<NewCode> NewCodes { get; set; }
        public List<NewZone> NewZones { get; set; }

        public List<RenamedZone> RenamedZones { get; set; }

        public List<DeletedZone> DeletedZones { get; set; }


    }
}
