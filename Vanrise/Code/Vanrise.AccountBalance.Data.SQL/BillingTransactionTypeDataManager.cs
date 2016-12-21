using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.AccountBalance.Data.SQL
{
    public class BillingTransactionTypeDataManager:BaseSQLDataManager,IBillingTransactionTypeDataManager
    {
       
        #region Constructors
        public BillingTransactionTypeDataManager()
            : base(GetConnectionStringName("VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString"))
        {

        }
         #endregion

        #region Public Methods

        public IEnumerable<BillingTransactionType> GetBillingTransactionTypes()
        {
            return GetItemsSP("VR_AccountBalance.sp_BillingTransactionType_GetAll", BillingTransactionTypeMapper);
        }

        public bool AreBillingTransactionTypeUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("VR_AccountBalance.BillingTransactionType", ref updateHandle);
        }

        #endregion
     
        #region Mappers

        private BillingTransactionType BillingTransactionTypeMapper(IDataReader reader)
        {
            return new BillingTransactionType
            {
                BillingTransactionTypeId = GetReaderValue<Guid>(reader,"ID"),
                Name = reader["Name"] as string,
                IsCredit = GetReaderValue<bool>(reader, "IsCredit"),
                Settings = Common.Serializer.Deserialize<BillingTransactionTypeSettings>(reader["Settings"] as string)
            };
        }

        #endregion
    }
}
