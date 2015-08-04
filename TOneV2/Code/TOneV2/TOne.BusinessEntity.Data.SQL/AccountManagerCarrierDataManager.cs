using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class AccountManagerCarrierDataManager : BaseTOneDataManager, IAccountManagerCarrierDataManager
    {
        public Vanrise.Entities.BigResult<AccountManagerCarrier> GetCarriers(Vanrise.Entities.DataRetrievalInput<AccountManagerCarrierQuery> input)
        {
            Dictionary<string, string> columnMapper = new Dictionary<string, string>();
            columnMapper.Add("CarrierName", "Name");

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("[BEntity].[sp_AccountManager_CreateTempForAccountManagerCarriers]", tempTableName, input.Query.UserId);
            };

            return RetrieveData(input, createTempTableAction, AccountManagerCarrier, columnMapper);
        }

        AccountManagerCarrier AccountManagerCarrier(IDataReader reader)
        {
            AccountManagerCarrier carrier = new AccountManagerCarrier
            {
                CarrierAccountId = reader["CarrierAccountId"] as string,
                CarrierName = GetCarrierName(reader["Name"] as string, reader["NameSuffix"] as string),
                IsCustomerAvailable = (bool)reader["IsCustomerAvailable"],
                IsSupplierAvailable = (bool)reader["IsSupplierAvailable"]
            };

            return carrier;
        }

        private string GetCarrierName(string carrierName, string nameSuffix)
        {
            string name = carrierName;

            if (nameSuffix != "")
                name += " (" + nameSuffix + ")";

            return name;
        }
    }
}
