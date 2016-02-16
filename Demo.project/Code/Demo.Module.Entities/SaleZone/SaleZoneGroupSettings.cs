using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities.BusinessEntities;

namespace Demo.Module.Entities
{
    public abstract class SaleZoneGroupSettings : Vanrise.GenericData.Entities.IBusinessEntityGroup
    {
        public int ConfigId { get; set; }

        public int SellingNumberPlanId { get; set; }

        public abstract IEnumerable<long> GetZoneIds(ISaleZoneGroupContext context);

        public abstract string GetDescription(ISaleZoneGroupContext context);

        IEnumerable<object> Vanrise.GenericData.Entities.IBusinessEntityGroup.GetIds(IBusinessEntityGroupContext context)
        {
            return this.GetZoneIds(null) as IEnumerable<Object>;
        }

        string Vanrise.GenericData.Entities.IBusinessEntityGroup.GetDescription(IBusinessEntityGroupContext context)
        {
            return this.GetDescription(null);
        }
    }
}
