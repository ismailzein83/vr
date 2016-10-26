using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities.BusinessEntities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class SaleZoneGroupSettings : Vanrise.GenericData.Entities.IBusinessEntityGroup
    {
        public abstract Guid ConfigId { get; }

        public int SellingNumberPlanId { get; set; }

        public abstract IEnumerable<long> GetZoneIds(ISaleZoneGroupContext context);

        public abstract void CleanDeletedZoneIds(ISaleZoneGroupCleanupContext context);

        public abstract string GetDescription(ISaleZoneGroupContext context);

        IEnumerable<object> Vanrise.GenericData.Entities.IBusinessEntityGroup.GetIds(IBusinessEntityGroupContext context)
        {
            return GetZoneIds(null).MapRecords(itm => (object)itm);
        }

        string Vanrise.GenericData.Entities.IBusinessEntityGroup.GetDescription(IBusinessEntityGroupContext context)
        {
            return this.GetDescription(null);
        }
    }
}
