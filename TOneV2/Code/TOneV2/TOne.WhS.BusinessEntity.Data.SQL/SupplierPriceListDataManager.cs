using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SupplierPriceListDataManager : BaseTOneDataManager, ISupplierPriceListDataManager
    {

        #region ctor/Local Variables
        public SupplierPriceListDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public Entities.SupplierPriceList GetPriceList(int priceListId)
        {
            return GetItemSP("TOneWhS_BE.sp_SupplierPriceList_Get", SupplierPriceListMapper, priceListId);
        }
        public List<SupplierPriceList> GetPriceLists()
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierPriceList_GetAll", SupplierPriceListMapper);
        }
        public bool ArGetPriceListsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SupplierPriceList", ref updateHandle);
        }

        public bool SavePriceList(int priceListStatus, DateTime effectiveOnDateTime, string supplierId, string priceListType, string activeSupplierEmail, byte[] contentBytes, string fileName, string messageUid, out int insertedId)
        {
            object id;
            int recordesEffected = ExecuteNonQuerySP("[sp_SupplierPriceList_Insert]", out id, priceListStatus, effectiveOnDateTime, supplierId, priceListType, activeSupplierEmail, contentBytes, fileName, messageUid);
            insertedId = (int)id;
            return (recordesEffected > 0);
        }
        #endregion

        #region Private Methods
        #endregion

        #region Mappers


        SupplierPriceList SupplierPriceListMapper(IDataReader reader)
        {
            SupplierPriceList supplierPriceList = new SupplierPriceList
            {
                SupplierId = (int)reader["SupplierID"],
                CurrencyId = (int)reader["CurrencyID"],
                PriceListId = (int)reader["ID"],
                FileId = (long)reader["FileID"]
            };
            return supplierPriceList;
        }

        #endregion
        public int GetQueueStatus(int queueId)
        {
            object result;
            ExecuteNonQuerySP("[sp_SupplierPriceList_GetResults]", out result, queueId);
            return (int)result;
        }
    }
}
