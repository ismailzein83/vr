using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Data.RDB;

namespace TOne.WhS.SMSBusinessEntity.Data
{
    public interface ISupplierSMSPriceListDataManager : IDataManager
    {
        void JoinRateTableWithPriceListTable(RDBJoinContext joinContext, string supplierPriceListTableAlias, string otherTableAlias, string otherTableColumn);

        void AddInsertPriceListQueryContext(RDBQueryContext queryContext, SupplierSMSPriceList supplierSMSPriceList);
    }
}
