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
   public  class CarrierAccountRouteFilter:ICarrierAccountFilter
    {

       public bool IsExcluded(ICarrierAccountFilterContext context)
       {
           CarrierAccountManager manager = new CarrierAccountManager();
           RouteCarrierAccountExtension routeCarrierAccountExtension = manager.GetExtendedSettings<RouteCarrierAccountExtension>(context.CarrierAccount);
           if (routeCarrierAccountExtension != null && routeCarrierAccountExtension.RouteInfo!=null && routeCarrierAccountExtension.RouteInfo.Count > 0)
               return false;
           return true;

       }
    }
}
