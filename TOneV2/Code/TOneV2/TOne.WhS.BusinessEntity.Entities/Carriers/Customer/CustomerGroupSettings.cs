using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class CustomerGroupSettings : Vanrise.GenericData.Entities.IBusinessEntityGroup
    {
        public int ConfigId { get; set; }

        public abstract IEnumerable<int> GetCustomerIds(ICustomerGroupContext context);

        public abstract string GetDescription(ICustomerGroupContext context);

        string Vanrise.GenericData.Entities.IBusinessEntityGroup.GetDescription(Vanrise.GenericData.Entities.BusinessEntities.IBusinessEntityGroupContext context)
        {
            return this.GetDescription(null);
        }

        IEnumerable<object> Vanrise.GenericData.Entities.IBusinessEntityGroup.GetIds(Vanrise.GenericData.Entities.BusinessEntities.IBusinessEntityGroupContext context)
        {
            return this.GetCustomerIds(null) as IEnumerable<object>;
        }
    }

}
