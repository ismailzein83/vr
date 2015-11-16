using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class AccountManagerCarrierDataManager : BaseTOneDataManager, IAccountManagerCarrierDataManager
    {

        public AccountManagerCarrierDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        public IEnumerable<AssignedCarrier> GetAssignedCarriers()
        {
            return GetItemsSP("[TOneWhS_BE].[sp_AccountManager_GetAll]", AssignedCarrierMapper);
        }


        AssignedCarrier AssignedCarrierMapper(IDataReader reader)
        {
            AssignedCarrier accountManager = new AssignedCarrier
            {
                CarrierAccountId = GetReaderValue<int>(reader,"CarrierAccountId") ,
                UserId = GetReaderValue<int>(reader, "UserId"),
                RelationType = (CarrierAccountType) reader["RelationType"]
            };

            return accountManager;
        }

        public bool AreAccountManagerUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.AccountManager", ref updateHandle);
        }
    }
}
