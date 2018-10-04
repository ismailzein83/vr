using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace NP.IVSwitch.Business
{
     public  class CarrierAccountEndPointFilter : ICarrierAccountFilter
    {
         public bool IsExcluded(ICarrierAccountFilterContext context)
        {
            CarrierAccountManager manager=new CarrierAccountManager();
            EndPointCarrierAccountExtension endPointCarrierAccountExtension = endPointCarrierAccountExtension= manager.GetExtendedSettings<EndPointCarrierAccountExtension>(context.CarrierAccount);
            if (endPointCarrierAccountExtension != null && ((endPointCarrierAccountExtension.AclEndPointInfo != null && endPointCarrierAccountExtension.AclEndPointInfo.Count > 0) || (endPointCarrierAccountExtension.UserEndPointInfo != null && endPointCarrierAccountExtension.UserEndPointInfo.Count > 0)))
            return false;
            return true;

        }
    
    }
}
