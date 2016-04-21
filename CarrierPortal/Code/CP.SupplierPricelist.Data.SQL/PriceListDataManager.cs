using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Data.SQL;
using CP.SupplierPricelist.Entities;
using System.Data;
using Vanrise.Common;
using Vanrise.Entities;
namespace CP.SupplierPricelist.Data.SQL
{
    public class PriceListDataManager : BaseSQLDataManager, IPriceListDataManager
    {
        private static Dictionary<string, string> _mapper = new Dictionary<string, string>();

        
        static PriceListDataManager()
        {
            _mapper.Add("CustomerName", "CustomerID");
            _mapper.Add("PriceListTypeValue", "PriceListType");
            _mapper.Add("PriceListStatusDescription", "Status");
            _mapper.Add("PriceListResultDescription", "Result");

        }
        public PriceListDataManager() :
            base(GetConnectionStringName("CP_DBConnStringKey", "CP_DBConnString"))
        {

        }
        public bool Insert(PriceList priceList, int resultIdtoBeExcluded, out int priceListId)
        {
            object id;
            int recordEffected = ExecuteNonQuerySP("[CP_SupPriceList].[sp_PriceList_Insert]", out id
                , priceList.UserId
                , priceList.FileId
                , priceList.PriceListType
                , 0
                , priceList.EffectiveOnDate
                , priceList.CustomerId
                , priceList.CarrierAccountId
                , resultIdtoBeExcluded
                , priceList.CarrierAccountName);
            priceListId = (int)id;
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
                AlertMessage = reader["AlertMessage"] as string,
                CustomerId = (int)reader["CustomerID"],
                AlertFileId = GetReaderValue<long>(reader, "AlertFileID"),
                CarrierAccountId = reader["CarrierAccountID"] as string,
                CarrierAccountName = GetReaderValue<string>(reader, "CarrierAccountName")
            };
            string uploadedInformationSerialized = reader["UploadInformation"] as string;
            if (uploadedInformationSerialized != null)
                pricelist.UploadedInformation = Serializer.Deserialize(uploadedInformationSerialized);

            string pricelistProgressSerialized = reader["PriceListProgress"] as string;
            if (pricelistProgressSerialized != null)
                pricelist.PriceListProgress = Serializer.Deserialize(pricelistProgressSerialized);

            return pricelist;
        }
               
        public BigResult<PriceList> GetPriceListsFilteredFromTemp(DataRetrievalInput<PriceListQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                string customersids = null;
                if (input.Query.CustomersIDs != null && input.Query.CustomersIDs.Count() > 0)
                    customersids = string.Join<int>(",", input.Query.CustomersIDs);

                string carrierids = null;
                if (input.Query.CarrierAccounts != null && input.Query.CarrierAccounts.Count() > 0)
                    carrierids = string.Join<string>(",", input.Query.CarrierAccounts);

                string types = null;
                if (input.Query.PriceListTypes != null && input.Query.PriceListTypes.Count() > 0)
                    types = string.Join<int>(",", input.Query.PriceListTypes);

                string results = null;
                if (input.Query.PriceListResults != null && input.Query.PriceListResults.Count() > 0)
                    results = string.Join<int>(",", input.Query.PriceListResults);

                string statuses = null;
                if (input.Query.PriceListStatuses != null && input.Query.PriceListStatuses.Count() > 0)
                    statuses = string.Join<int>(",", input.Query.PriceListStatuses);


                ExecuteNonQuerySP("[CP_SupPriceList].[sp_PriceList_GetTempByFiltred]",
                    tempTableName,
                    input.Query.FromEffectiveOnDate,
                    input.Query.ToEffectiveOnDate,
                    input.Query.UserId ,
                    customersids,
                    carrierids,
                    types,
                    results,
                    statuses
                   );
            };

            if (input.SortByColumnName != null)
                input.SortByColumnName = input.SortByColumnName.Replace("Entity.", "");

            return RetrieveData(input, createTempTableAction, PriceListMapper ,_mapper );
        }

        public List<PriceList> GetPriceLists(List<PriceListStatus> listStatuses)
        {
            string pricelistStatuIds = null;
            if (listStatuses != null && listStatuses.Any())
                pricelistStatuIds = string.Join(",", Array.ConvertAll(listStatuses.ToArray(), value => (int)value));
            return GetItemsSP("[CP_SupPriceList].[sp_PriceList_GetRequestedPriceList]", PriceListMapper, pricelistStatuIds);
        }
        public List<PriceList> GetPriceLists(List<PriceListStatus> listStatuses, int customerId)
        {
            string pricelistStatuIds = null;
            if (listStatuses != null && listStatuses.Any())
                pricelistStatuIds = string.Join(",", Array.ConvertAll(listStatuses.ToArray(), value => (int)value));
            return GetItemsSP("[CP_SupPriceList].[sp_PriceList_GetRequestedPriceList]", PriceListMapper, pricelistStatuIds, customerId);
        }

        public bool UpdatePriceListUpload(long id, int result, int status, object uploadInformation, int uploadRetryCount)
        {
            int recordsEffected = ExecuteNonQuerySP("[CP_SupPriceList].[sp_PriceList_UpdateInitiatePriceList]",
                id, result, status, uploadInformation != null ? Serializer.Serialize(uploadInformation) : null, uploadRetryCount);
            return (recordsEffected > 0);
        }


        public bool UpdatePriceListProgress(long id, int status, int result, int resultRetryCount, string alertMessage, long alertFileId)
        {
            int recordsEffected = ExecuteNonQuerySP("[CP_SupPriceList].[sp_PriceList_UpdatePriceListProgress]", id, status, result, resultRetryCount, alertMessage, alertFileId);
            return (recordsEffected > 0);
        }


        public List<PriceList> GetBeforeId(GetBeforeIdInput input)
        {
            return GetItemsSP("[CP_SupPriceList].[sp_PriceList_GetBeforeID]", PriceListMapper, input.LessThanID, input.NbOfRows, input.UserId);
        }
    }
}
