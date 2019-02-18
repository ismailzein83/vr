using System.Collections.Generic;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Data.RDB;

namespace TOne.WhS.SMSBusinessEntity.Data
{
    public interface ISupplierSMSPriceListDataManager : IDataManager
    {
        List<SupplierSMSPriceList> GetSupplierSMSPriceLists();

        void JoinRateTableWithPriceListTable(RDBJoinContext joinContext, string supplierPriceListTableAlias, string otherTableAlias, string otherTableColumn);

        bool AreSupplierSMSPriceListUpdated(ref object updateHandle);

        void AddInsertPriceListQueryContext(RDBQueryContext queryContext, SupplierSMSPriceList supplierSMSPriceList);
    }
}
