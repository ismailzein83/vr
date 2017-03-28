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

        public List<VRNotification> GetNotClearedNotifications(Guid notificationTypeId, VRNotificationParentTypes parentTypes, List<string> eventKeys, DateTime? notificationCreatedAfter)
        {
            string eventKeysAsString = null;
            if (eventKeys != null && eventKeys.Count > 0)
                eventKeysAsString = string.Join<string>(",", eventKeys);

            string parentType1 = null;
            string parentType2 = null;
            if (parentTypes != null)
            {
                parentType1 = parentTypes.ParentType1;
                parentType2 = parentTypes.ParentType2;
            }
            return GetItemsSP("[VRNotification].sp_VRNotification_GetNotCleared", VRNotificationMapper, notificationTypeId, parentType1, parentType2, eventKeysAsString, notificationCreatedAfter);
        }

        public void GetFirstPageVRNotifications(IVRNotificationFirstPageContext context)
        {
            bool isFinalRow = false;

            string description = null;
            if (context.Query != null && !string.IsNullOrEmpty(context.Query.Description))
                description = context.Query.Description;

            string statusIds = null;
            if (context.Query != null && context.Query.StatusIds != null && context.Query.StatusIds.Count > 0)
                statusIds = string.Join<int>(",", context.Query.StatusIds);

            string alerttLevelIds = null;
            if (context.Query != null && context.Query.AlertLevelIds != null && context.Query.AlertLevelIds.Count > 0)
                alerttLevelIds = string.Join<Guid>(",", context.Query.AlertLevelIds);

            ExecuteReaderSP("[VRNotification].[sp_VRNotifications_GetFirstPage]", (reader) =>
            {
                while (reader.Read() && !isFinalRow)
                    isFinalRow = context.onItemReady(VRNotificationMapper(reader));

                if (reader.NextResult())
                    while (reader.Read())
                        context.MaxTimeStamp = GetReaderValue<byte[]>(reader, "MaxTimestamp");

            }, context.NotificationTypeId, context.NbOfRows, description, statusIds, alerttLevelIds);
        }

        public List<VRNotification> GetUpdateVRNotifications(IVRNotificationUpdateContext context)
        {
            string description = null;
            if (context.Query != null && !string.IsNullOrEmpty(context.Query.Description))
                description = context.Query.Description;

            string alerttLevelIds = null;
            if (context.Query != null && context.Query.AlertLevelIds != null && context.Query.AlertLevelIds.Count > 0)
                alerttLevelIds = string.Join<Guid>(",", context.Query.AlertLevelIds);

            List<VRNotification> notifications = new List<VRNotification>();
            ExecuteReaderSP("[VRNotification].[sp_VRNotifications_GetUpdated]", (reader) =>
            {
                while (reader.Read())
                    notifications.Add(VRNotificationMapper(reader));

                if (reader.NextResult())
                    while (reader.Read())
                        context.MaxTimeStamp = GetReaderValue<byte[]>(reader, "MaxTimestamp");

            }, context.NotificationTypeId, context.NbOfRows, context.MaxTimeStamp, description, alerttLevelIds);

            return notifications;
        }

        public void GetBeforeIdVRNotifications(IVRNotificationBeforeIdContext context)
        {
            bool isFinalRow = false;

            string description = null;
            if (context.Query != null && !string.IsNullOrEmpty(context.Query.Description))
                description = context.Query.Description;

            string statusIds = null;
            if (context.Query != null && context.Query.StatusIds != null && context.Query.StatusIds.Count > 0)
                statusIds = string.Join<int>(",", context.Query.StatusIds);

            string alerttLevelIds = null;
            if (context.Query != null && context.Query.AlertLevelIds != null && context.Query.AlertLevelIds.Count > 0)
                alerttLevelIds = string.Join<Guid>(",", context.Query.AlertLevelIds);

            ExecuteReaderSP("[VRNotification].[sp_VRNotifications_GetBeforeID]", (reader) =>
            {
                while (reader.Read() && !isFinalRow)
                    isFinalRow = context.onItemReady(VRNotificationMapper(reader));

            }, context.NotificationTypeId, context.NbOfRows, context.LessThanID, description, statusIds, alerttLevelIds);
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