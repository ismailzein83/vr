using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities.CP
{
    public class Changes
    {
        public Changes()
        {
            DeletedCodes = new List<DeletedCode>();
            NewCodes = new List<NewCode>();
            NewZones = new List<NewZone>();
        }
        public List<DeletedCode> DeletedCodes { get; set; }
        public List<NewCode> NewCodes { get; set; }
        public List<NewZone> NewZones { get; set; }

    }
}
