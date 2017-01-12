using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;


namespace Retail.BusinessEntity.Data.SQL
{
    public class CreditClassDataManager : BaseSQLDataManager, ICreditClassDataManager
    {
        #region ctor/Local Variables
        public CreditClassDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }
        #endregion

        #region Public Methods

        public List<CreditClass> GetCreditClasses()
        {
            return GetItemsSP("[Retail_BE].[sp_CreditClass_GetAll]", CreditClassMapper);
        }

        public bool AreCreditClassUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[Retail_BE].[CreditClass]", ref updateHandle);
        }

        public bool Insert(CreditClass CreditClassItem, out int insertedId)
        {
            object creditClassID;
            string serializedSettings = CreditClassItem.Settings != null ? Vanrise.Common.Serializer.Serialize(CreditClassItem.Settings) : null;

            int affectedRecords = ExecuteNonQuerySP("[Retail_BE].[sp_CreditClass_Insert]", out creditClassID, CreditClassItem.Name, serializedSettings);

            insertedId = (affectedRecords > 0) ? (int)creditClassID : -1;
            return (affectedRecords > 0);
        }

        public bool Update(CreditClass CreditClassItem)
        {
            string serializedSettings = CreditClassItem.Settings != null ? Vanrise.Common.Serializer.Serialize(CreditClassItem.Settings) : null;

            int affectedRecords = ExecuteNonQuerySP("[Retail_BE].[sp_CreditClass_Update]", CreditClassItem.CreditClassId, CreditClassItem.Name, serializedSettings);
            return (affectedRecords > 0);
        }

        #endregion

        #region Mappers

        CreditClass CreditClassMapper(IDataReader reader)
        {
            CreditClass CreditClass = new CreditClass
            {
                CreditClassId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<CreditClassSettings>(reader["Settings"] as string)
            };
            return CreditClass;
        }

        #endregion
    }
}
