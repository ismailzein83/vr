using System;
using System.Collections.Generic;
using System.Data;
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

        public VRNotification GetVRNotificationById(long notificationId)
        {
            return GetItemSP("[VRNotification].[sp_VRNotification_GetById]", VRNotificationMapper, notificationId);
        }

        public bool Insert(VRNotification notification, out long notificationId)
        {
            object insertedId;
            int affectedRecords = ExecuteNonQuerySP("VRNotification.sp_VRNotification_Insert", out insertedId,
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
            notificationId = (long)insertedId;
            return (affectedRecords > 0);
        }

        public List<VRNotification> GetVRNotifications(Guid notificationTypeId, VRNotificationParentTypes parentTypes, string eventKey)
        {
            return GetItemsSP("VRNotification.sp_VRNotification_GetByNotificationType", VRNotificationMapper, notificationTypeId, parentTypes.ParentType1, parentTypes.ParentType2, eventKey);
        }

        public void UpdateNotificationStatus(long notificationId, VRNotificationStatus vrNotificationStatus)
        {
            ExecuteNonQuerySP("[VRNotification].[sp_VRNotification_UpdateStatus]", notificationId, vrNotificationStatus);
        }

        public void GetFirstPageVRNotifications(VRNotificationFirstPageInput input, long? nbOfRows, ref byte[] maxTimeStamp, Func<VRNotification, bool> onItemReady)
        {
            byte[] timestamp = null;
            bool isFinalRow = false;

            if (!nbOfRows.HasValue)
                nbOfRows = long.MaxValue;

            object description = DBNull.Value;
            if (input.Query != null && !string.IsNullOrEmpty(input.Query.Description))
                description = input.Query.Description;

            object statusIds = DBNull.Value;
            if (input.Query != null && input.Query.StatusIds != null && input.Query.StatusIds.Count > 0)
                statusIds = string.Join<int>(",", input.Query.StatusIds);

            object alerttLevelIds = DBNull.Value;
            if (input.Query != null && input.Query.AlertLevelIds != null && input.Query.AlertLevelIds.Count > 0)
                alerttLevelIds = string.Join<Guid>(",", input.Query.AlertLevelIds);

            ExecuteReaderSP("[VRNotification].[sp_VRNotifications_GetFirstPage]", (reader) =>
            {
                while (reader.Read() && !isFinalRow)
                    isFinalRow = onItemReady(VRNotificationMapper(reader));

                if (reader.NextResult())
                    while (reader.Read())
                        timestamp = GetReaderValue<byte[]>(reader, "MaxTimestamp");

            }, input.NotificationTypeId, nbOfRows.Value, description, statusIds, alerttLevelIds);

            maxTimeStamp = timestamp;
        }

        public void GetUpdateVRNotifications(VRNotificationUpdateInput input, ref byte[] maxTimeStamp, Action<VRNotification> onItemReady)
        {
            byte[] timestamp = null;

            object description = DBNull.Value;
            if (input.Query != null && !string.IsNullOrEmpty(input.Query.Description))
                description = input.Query.Description;

            object alerttLevelIds = DBNull.Value;
            if (input.Query != null && input.Query.AlertLevelIds != null && input.Query.AlertLevelIds.Count > 0)
                alerttLevelIds = string.Join<Guid>(",", input.Query.AlertLevelIds);

            ExecuteReaderSP("[VRNotification].[sp_VRNotifications_GetUpdated]", (reader) =>
            {
                while (reader.Read())
                    onItemReady(VRNotificationMapper(reader));

                if (reader.NextResult())
                    while (reader.Read())
                        timestamp = GetReaderValue<byte[]>(reader, "MaxTimestamp");

            }, input.NotificationTypeId, input.NbOfRows, input.LastUpdateHandle, description, alerttLevelIds);

            maxTimeStamp = timestamp;
        }

        public void GetBeforeIdVRNotifications(VRNotificationBeforeIdInput input, long? nbOfRows, Func<VRNotification, bool> onItemReady)
        {
            bool isFinalRow = false;

            if (!nbOfRows.HasValue)
                nbOfRows = long.MaxValue;

            object description = DBNull.Value;
            if (input.Query != null && !string.IsNullOrEmpty(input.Query.Description))
                description = input.Query.Description;

            object statusIds = DBNull.Value;
            if (input.Query != null && input.Query.StatusIds != null && input.Query.StatusIds.Count > 0)
                statusIds = string.Join<int>(",", input.Query.StatusIds);

            object alerttLevelIds = DBNull.Value;
            if (input.Query != null && input.Query.AlertLevelIds != null && input.Query.AlertLevelIds.Count > 0)
                alerttLevelIds = string.Join<Guid>(",", input.Query.AlertLevelIds);

            ExecuteReaderSP("[VRNotification].[sp_VRNotifications_GetBeforeID]", (reader) =>
            {
                while (reader.Read() && !isFinalRow)
                    isFinalRow = onItemReady(VRNotificationMapper(reader));

            }, input.NotificationTypeId, nbOfRows.Value, input.LessThanID, description, statusIds, alerttLevelIds);
        }

        #endregion

        #region Mappers

        VRNotification VRNotificationMapper(IDataReader reader)
        {

            return new VRNotification
            {
                VRNotificationId = (long)reader["ID"],
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
                CreationTime = (DateTime)reader["CreationTime"],
                Data = Serializer.Deserialize<VRNotificationData>(reader["Data"] as string)
            };
        }

        #endregion



    }
}
