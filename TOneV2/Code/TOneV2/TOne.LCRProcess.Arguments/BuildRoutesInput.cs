using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.LCR.Entities;
using Vanrise.Queueing;

namespace TOne.LCRProcess.Arguments
{
    public class BuildRoutesInput
    {
        public List<CarrierAccountInfo> ActiveSuppliers { get; set; }
        public string CodePrefixGroup { get; set; }

        public bool IsFuture { get; set; }

        public DateTime EffectiveTime { get; set; }

        public BaseQueue<List<CodeMatch>> CodeMatchesQueue { get; set; }
    }
}
