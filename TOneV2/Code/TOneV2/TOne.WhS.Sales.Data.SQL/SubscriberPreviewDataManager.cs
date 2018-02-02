using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Sales.Data.SQL
{
    public class SubscriberPreviewDataManager : BaseSQLDataManager, ISubscriberPreviewDataManager
    {
        private long _processInstanceId;

        public long ProcessInstanceId
        {
            set
            {
                _processInstanceId = value;
            }
        }


        #region Constructors

        public SubscriberPreviewDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        public IEnumerable<SubscriberPreview> GetSubscriberPreviews(long processInstanceId)
        {
            return GetItemsSP("TOneWhS_Sales.sp_SubscriberPreview_Get", SubscriberPreviewMapper, processInstanceId);
        }

        public bool InsertSubscriberPreview(SubscriberPreview subscriberPreview)
        {
            int affectedRows = ExecuteNonQuerySP("TOneWhS_Sales.sp_SubscriberPreview_Insert", _processInstanceId, subscriberPreview.SubscriberId, Convert.ToInt32(subscriberPreview.Status), subscriberPreview.Description);

            return affectedRows > 0;
        }

        private SubscriberPreview SubscriberPreviewMapper(IDataReader reader)
        {
            return new SubscriberPreview()
            {
                SubscriberId = GetReaderValue<int>(reader, "SubscriberID"),
                Status = (SubscriberProcessStatus)reader["Status"],
                Description = GetReaderValue<string>(reader, "Description")
            };
        }
    }
}
