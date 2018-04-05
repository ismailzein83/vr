using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
   public class CodesByZoneOtherRatesMappedValue : CodesByZoneMappedValue
    {
        public override Guid ConfigId
        {
            get { return new Guid("A2E3EBFB-15DC-42AB-B410-AD3328A89E32"); }
        }

        public int RateTypeId { get; set; }
        public override void Execute(ICodesByZoneMappedValueContext context)
        {
            context.Value = null;
        }
        
    }
}
