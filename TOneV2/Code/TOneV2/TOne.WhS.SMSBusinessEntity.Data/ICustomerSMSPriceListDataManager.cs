using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Data.RDB;

namespace TOne.WhS.SMSBusinessEntity.Data
{
    public interface ICustomerSMSPriceListDataManager
    {
        void Join(RDBSelectQuery selectQuery, string firstTableAlias, string firstTableColumn, int customerID);
    }
}
