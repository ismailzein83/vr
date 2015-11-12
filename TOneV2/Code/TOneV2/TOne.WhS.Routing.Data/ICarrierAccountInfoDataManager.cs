using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data
{
    public interface ICarrierAccountInfoDataManager : IDataManager, IRoutingDataManager
    {
        IEnumerable<RoutingCustomerInfo> GetRoutingtCustomerInfo();
        IEnumerable<RoutingSupplierInfo> GetRoutingtSupplierInfo();
        void ApplyRoutingCustomerInfoToDB(Object preparedCustomerInfos);
        void ApplyRoutingSupplierInfoToDB(Object preparedSupplierInfos);
        Object PrepareRoutingCustomerInfoForDBApply(List<RoutingCustomerInfo> customerInfos);
        Object PrepareRoutingSupplierInfoForDBApply(List<RoutingSupplierInfo> supplierInfos);

    }
}
