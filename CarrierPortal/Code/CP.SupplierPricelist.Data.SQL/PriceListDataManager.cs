using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Data.SQL;
using CP.SupplierPricelist.Entities;
using System.Data;
using Vanrise.Common;

namespace CP.SupplierPricelist.Data.SQL
{
    public class PriceListDataManager : BaseSQLDataManager, IPriceListDataManager
    {
        public PriceListDataManager() :
            base(GetConnectionStringName("CP_DBConnStringKey", "CP_DBConnString"))
        {

        }

        public bool Insert(PriceList priceList)
        {
            int recordEffected = ExecuteNonQuerySP("[CP_SupPriceList].[sp_PriceList_Insert]", priceList.UserId, priceList.FileId, priceList.PriceListType, 0, priceList.EffectiveOnDate);
            return (recordEffected > 0);
        }


        public List<PriceList> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, int userId)
        {
            List<PriceList> listPriceLists = new List<PriceList>();
            byte[] timestamp = null;

            ExecuteReaderSP("CP_SupPriceList.sp_PriceList_GetUpdated", (reader) =>
            {
                while (reader.Read())
                    listPriceLists.Add(PriceListMapper(reader));
                if (reader.NextResult())
                    while (reader.Read())
                        timestamp = GetReaderValue<byte[]>(reader, "MaxTimestamp");
            },
               maxTimeStamp, nbOfRows, userId);
            maxTimeStamp = timestamp;
            return listPriceLists;
        }


        PriceList PriceListMapper(IDataReader reader)
        {
            PriceList pricelist = new PriceList
            {
                PriceListId = (long)reader["ID"],
                FileId = (long)reader["FileID"],
                PriceListType = GetReaderValue<PriceListType>(reader, "PriceListType"),
                Status = GetReaderValue<PriceListStatus>(reader, "Status"),
                UserId = (int)reader["UserID"],
                Result = GetReaderValue<PriceListResult>(reader, "Result"),
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                EffectiveOnDate = GetReaderValue<DateTime>(reader, "EffectiveOnDate"),
                ResultMaxRetryCount = GetReaderValue<int>(reader, "ResultRetryCount"),
                UploadMaxRetryCount = GetReaderValue<int>(reader, "UploadRetryCount"),
                AlertMessage = reader["AlertMessage"] as string
            };
            string uploadedInformationSerialized = reader["UploadInformation"] as string;
            if (uploadedInformationSerialized != null)
                pricelist.UploadedInformation = Serializer.Deserialize(uploadedInformationSerialized);

            string pricelistProgressSerialized = reader["PriceListProgress"] as string;
            if (pricelistProgressSerialized != null)
                pricelist.PriceListProgress = Serializer.Deserialize(pricelistProgressSerialized);

            return pricelist;
        }


        public List<PriceList> GetPriceLists(List<PriceListStatus> listStatuses)
        {
            string pricelistStatuIds = null;
            if (listStatuses != null && listStatuses.Any())
                pricelistStatuIds = string.Join(",", Array.ConvertAll(listStatuses.ToArray(), value => (int)value));
            return GetItemsSP("[CP_SupPriceList].[sp_PriceList_GetRequestedPriceList]", PriceListMapper, pricelistStatuIds);
        }

        public bool UpdatePriceListUpload(long id, int result, object uploadInformation, int uploadRetryCount)
        {
            int recordsEffected = ExecuteNonQuerySP("[CP_SupPriceList].[sp_PriceList_UpdateInitiatePriceList]",
                id, result, uploadInformation != null ? Serializer.Serialize(uploadInformation) : null, uploadRetryCount);
            return (recordsEffected > 0);
        }


        public bool UpdatePriceListProgress(long id, int status, int result, int resultRetryCount, string alertMessage)
        {
            int recordsEffected = ExecuteNonQuerySP("[CP_SupPriceList].[sp_PriceList_UpdatePriceListProgress]", id, status, result, resultRetryCount, alertMessage);
            return (recordsEffected > 0);
        }
    }
}
