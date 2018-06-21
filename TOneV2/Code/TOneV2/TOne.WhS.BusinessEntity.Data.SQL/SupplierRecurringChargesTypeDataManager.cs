using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    class SupplierRecurringChargesTypeDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISupplierRecurringChargesTypeDataManager
    {
        #region ctor/Local Variables
        public SupplierRecurringChargesTypeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public List<SupplierRecurringChargesType> GetAllSupplierRecurringChargesTypes()
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierRecurringChargesType_GetAll", SupplierRecurringChargesTypeMapper);
        }

        public bool AreAllSupplierRecurringChargesTypesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[TOneWhS_BE].[SupplierRecurringChargesType]", ref updateHandle);
        }

        #endregion

        #region Mappers

        private SupplierRecurringChargesType SupplierRecurringChargesTypeMapper(IDataReader reader)
        {
            SupplierRecurringChargesType supplierRecurringChargesType = new SupplierRecurringChargesType();

            supplierRecurringChargesType.SupplierRecurringChargeTypeId = (long)reader["ID"];
            supplierRecurringChargesType.Name = (string)reader["Name"];

            return supplierRecurringChargesType;
        }

        #endregion
    }
}
