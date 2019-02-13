using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Data.RDB;

namespace TOne.WhS.SMSBusinessEntity.Data
{
    public interface ICustomerSMSPriceListDataManager : IDataManager
    {
        void JoinRateTableWithPriceListTable(RDBJoinContext joinContext, string customerPriceListTableAlias, string otherTableAlias, string otherTableColumn);

        void AddInsertPriceListQueryContext(RDBQueryContext queryContext, CustomerSMSPriceList customerSMSPriceList);
    }
}
