using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class SupplierPricelistsDataManager : BaseTOneDataManager, ISupplierPricelistsDataManager
    {
        public Vanrise.Entities.BigResult<PriceLists> GetSupplierPriceLists(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("[BEntity].[sp_PriceList_GetBySupplierID]", tempTableName, input.Query);
            };

            return RetrieveData(input, createTempTableAction, SupplierPriceListsMapper);
        }
        private PriceLists SupplierPriceListsMapper(IDataReader reader)
        {
            PriceLists suppluerPriceLists = new PriceLists
            {
                PriceListID = (int)reader["PriceListID"],
                Description = reader["Description"] as string,
                SourceFileName = reader["SourceFileName"] as string,
                UserID = GetReaderValue<int>(reader, "UserID"),
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BeginEffectiveDate"),
                Name = reader["Name"] as string,
            };

            return suppluerPriceLists;
        }

        public int GetQueueStatus(int queueId)
        {
            object result;
            ExecuteNonQuerySP("[sp_SupplierPriceList_GetResults]", out result, queueId);
            return (int)result;
        }

        public bool SavePriceList(int priceListStatus, DateTime effectiveOnDateTime, string supplierId, string priceListType, string activeSupplierEmail, byte[] contentBytes, string fileName, string messageUid, out int insertedId)
        {
            object id;
            int recordesEffected = ExecuteNonQuerySP("[sp_SupplierPriceList_Insert]", out id, priceListStatus, effectiveOnDateTime, supplierId, priceListType, activeSupplierEmail, contentBytes, fileName, messageUid);
            insertedId = (int)id;
            return (recordesEffected > 0);
        }

        public UploadInfo GetUploadInfo(int queueId)
        {
            return GetItemSP("sp_SupplierPriceList_GetUploadInfo", UploadInfoMapper, queueId);
        }
        private UploadInfo UploadInfoMapper(IDataReader reader)
        {
            UploadInfo uploadInfo = new UploadInfo
            {
                Status = (int)reader["Status"]
            };
            if (reader["PricelistImportLog"] != null) uploadInfo.ContentBytes = (byte[])reader["PricelistImportLog"];
            if (reader["FileName"] != null) uploadInfo.FileName = reader["FileName"] as string;
            return uploadInfo;
        }
    }
}
