using System.Collections.Generic;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Data.RDB;

namespace TOne.WhS.SMSBusinessEntity.Data
{
    public interface ICustomerSMSPriceListDataManager : IDataManager
    {

        List<CustomerSMSPriceList> GetCustomerSMSPriceLists();

        void JoinRateTableWithPriceListTable(RDBJoinContext joinContext, string customerPriceListTableAlias, string otherTableAlias, string otherTableColumn);

        void AddInsertPriceListQueryContext(RDBQueryContext queryContext, CustomerSMSPriceList customerSMSPriceList);

        bool AreCustomerSMSPriceListUpdated(ref object updateHandle);
    }
}
