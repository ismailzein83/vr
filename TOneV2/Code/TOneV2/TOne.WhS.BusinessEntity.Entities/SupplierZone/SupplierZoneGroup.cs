using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vanrise.Common;
using Vanrise.GenericData.Entities.BusinessEntities;
namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class SupplierZoneGroup : Vanrise.GenericData.Entities.IBusinessEntityGroup
    {
        public virtual Guid ConfigId { get; set; }
        public abstract IEnumerable<long> GetSupplierZoneIds(ISupplierZoneGroupContext context);
        public abstract string GetDescription(ISupplierZoneGroupContext context);
        IEnumerable<object> Vanrise.GenericData.Entities.IBusinessEntityGroup.GetIds(IBusinessEntityGroupContext context)
        {
            return GetSupplierZoneIds(null).MapRecords(itm => (object)itm);
        }
        string Vanrise.GenericData.Entities.IBusinessEntityGroup.GetDescription(IBusinessEntityGroupContext context)
        {
            return this.GetDescription(null);
        }
    }
}
