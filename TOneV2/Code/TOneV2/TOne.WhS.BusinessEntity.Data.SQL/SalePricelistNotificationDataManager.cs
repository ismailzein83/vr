using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SalePricelistNotificationDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISalePricelistNotificationDataManager
    {
        #region ctor/Local Variables
        public SalePricelistNotificationDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        #endregion

        #region public Methods

        public bool Insert(int customerId, int pricelistId, long fileId)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SalePricelistNotification_Insert", customerId, pricelistId, fileId);
            return recordsEffected > 0;
        }
        public IEnumerable<SalePricelistNotification> GetSalePricelistNotifications()
        {
            return GetItemsSP("TOneWhS_BE.sp_SalePricelistNotification_GetAll", SalePricelistNotificationMapper);
        }

        public Dictionary<int, int> GetNotificationCountByPricelistId()
        {
            var salePricelistNotificationByPricelistId = new Dictionary<int, int>();
            ExecuteReaderSP("TOneWhS_BE.sp_SalePricelistNotification_GetByPlID", (reader) =>
            {
                while (reader.Read())
                {
                    int pricelistId = (int)reader["pricelistid"];
                    int notificationCount = (int)reader["notificationCount"];
                    salePricelistNotificationByPricelistId.Add(pricelistId, notificationCount);
                }
            });
            return salePricelistNotificationByPricelistId;
        }
        public bool AreSalePriceListNotificationUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SalePricelistNotification", ref updateHandle);
        }

        public IEnumerable<SalePricelistNotification> GetLastSalePricelistNotifications(IEnumerable<int> customerIds)
        {
            string customerIdsStr = "";
            if (customerIds != null && customerIds.Count() > 0)
                customerIdsStr = String.Join(",", customerIds.ToArray());
            return GetItemsSP("TOneWhS_BE.sp_SalePricelistNotification_GetLastNotification", SalePricelistNotificationMapper, customerIdsStr);
        }
        public IEnumerable<SalePricelistNotification> GetSalePricelistNotifictaions(int pricelistId)
        {
            return GetItemsSP("TOneWhS_BE.sp_SalePricelistNotification_Get", SalePricelistNotificationMapper, pricelistId);
        }
        #endregion

        #region Mappers
        SalePricelistNotification SalePricelistNotificationMapper(IDataReader reader)
        {
            return new SalePricelistNotification
            {
                CustomerID = (int)reader["CustomerID"],
                PricelistId = (int)reader["PricelistId"],
                FileId = (long)reader["FileID"],
                EmailCreationDate = GetReaderValue<DateTime>(reader, "EmailCreationDate"),
            };
        }

        #endregion
    }
}
