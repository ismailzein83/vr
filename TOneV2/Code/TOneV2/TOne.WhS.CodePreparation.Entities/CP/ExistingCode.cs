using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities.CP
{
    public class ExistingZone
    {
        public BusinessEntity.Entities.SaleZone ZoneEntity { get; set; }
    }

    public class ExistingCode //: Vanrise.Entities.IDateEffectiveSettings
    {
        public BusinessEntity.Entities.SaleCode CodeEntity { get; set; }
    }

    public class ChangedZone
    {
        public long ZoneId { get; set; }

        public DateTime EED { get; set; }
    }

    //public class ChangedCode
    //{
    //    public long CodeId { get; set; }

    //    public DateTime EED { get; set; }
    //}
}
