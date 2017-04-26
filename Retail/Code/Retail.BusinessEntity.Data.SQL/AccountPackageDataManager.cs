using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class AccountPackageDataManager : BaseSQLDataManager, IAccountPackageDataManager
    {
        #region Constructors

        public AccountPackageDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public IEnumerable<AccountPackage> GetAccountPackages()
        {
            return GetItemsSP("Retail.sp_AccountPackage_GetAll", AccountPackageMapper);
        }

        public bool Insert(AccountPackage accountPackage, out int insertedId)
        {
            object accountPackageId;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_AccountPackage_Insert", out accountPackageId, accountPackage.AccountId, accountPackage.PackageId, accountPackage.BED, accountPackage.EED);

            if (affectedRecords > 0)
            {
                insertedId = (int)accountPackageId;
                return true;
            }

            insertedId = -1;
            return false;
        }
        public bool Update(AccountPackageToEdit accountPackage)
        {
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_AccountPackage_Update", accountPackage.AccountPackageId, accountPackage.BED, accountPackage.EED);
            return (affectedRecords > 0);
        }
        public bool AreAccountPackagesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail.AccountPackage", ref updateHandle);
        }

        #endregion

        #region Mappers

        private AccountPackage AccountPackageMapper(IDataReader reader)
        {
            return new AccountPackage()
            {
                AccountPackageId = (int)reader["ID"],
                AccountId = (int)reader["AccountID"],
                PackageId = (int)reader["PackageID"],
                BED = (DateTime)reader["BED"],
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };
        }

        #endregion
    }
}
