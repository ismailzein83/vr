using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Data.SQL
{
    public class VRNotificationDataManager : BaseSQLDataManager, IVRNotificationDataManager
    {
        #region ctor/Local Variables
        public VRNotificationDataManager()
            : base(GetConnectionStringName("VRNotificationTransactionDBConnStringKey", "VRNotificationTransactionDBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public bool Insert(VRNotification notification)
        {
            int affectedRecords = ExecuteNonQuerySP("VRNotification.sp_VRNotification_Insert", notification.VRNotificationId,
                                                                                            notification.UserId,
                                                                                            notification.TypeId,
                                                                                            notification.ParentTypes.ParentType1,
                                                                                            notification.ParentTypes.ParentType2,
                                                                                            notification.EventKey,
                                                                                            notification.BPProcessInstanceId,
                                                                                            notification.Status,
                                                                                            notification.AlertLevelId,
                                                                                            notification.Description,
                                                                                            notification.ErrorMessage,
                                                                                            notification.Data != null ? Serializer.Serialize(notification.Data) : null);

            return (affectedRecords > 0);
        }

        public List<VRNotification> GetVRNotifications()
        {
            return GetItemsSP("VRNotification.sp_VRNotification_GetAll", VRNotificationMapper);
        }
        public void UpdateNotificationStatus(Guid notificationId, VRNotificationStatus vrNotificationStatus)
        {
            ExecuteNonQuerySP("[VRNotification].[sp_VRNotification_UpdateStatus]", notificationId, vrNotificationStatus);
        }

        public bool AreVRNotificationUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("VRNotification.VRNotification", ref updateHandle);
        }

        #endregion

        #region Mappers
        VRNotification VRNotificationMapper(IDataReader reader)
        {

            return new VRNotification
            {
                VRNotificationId = GetReaderValue<Guid>(reader, "ID"),
                UserId = (int)reader["UserID"],
                ParentTypes = new VRNotificationParentTypes
                {
                    ParentType1 = reader["ParentType1"] as string,
                    ParentType2 = reader["ParentType2"] as string
                },
                Status = GetReaderValue<VRNotificationStatus>(reader, "Status"),
                EventKey = reader["EventKey"] as string,
                TypeId = GetReaderValue<Guid>(reader, "TypeID"),
                AlertLevelId = GetReaderValue<Guid>(reader, "AlertLevelID"),
                Description = reader["Description"] as string,
                BPProcessInstanceId = GetReaderValue<long?>(reader, "BPProcessInstanceID"),
                ErrorMessage = reader["ErrorMessage"] as string,
                Data = Serializer.Deserialize<VRNotificationData>(reader["Data"] as string)
            };
        }

        #endregion
    }
}
