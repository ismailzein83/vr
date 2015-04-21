using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class RoutePoolSelectionSet : BaseCarrierAccountSet
    {
        public int RoutePoolId { get; set; }

        public override string GetDescription(BusinessEntity.Entities.IBusinessEntityInfoManager businessEntityManager)
        {
            throw new NotImplementedException();
        }
    }
}
