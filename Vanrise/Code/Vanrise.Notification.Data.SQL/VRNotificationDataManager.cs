using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.Notification.Entities;
using System.Linq;

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

            int affectedRecords = ExecuteNonQuerySP("VRNotification.sp_VRNotification_Insert", out insertedId, notification.UserId, notification.TypeId,
                notification.ParentTypes.ParentType1, notification.ParentTypes.ParentType2, notification.EventKey, notification.Status,
                notification.AlertLevelId, notification.Description, notification.ErrorMessage, notification.Data != null ? Serializer.Serialize(notification.Data) : null,
                notification.EventPayload != null ? Serializer.Serialize(notification.EventPayload) : null);

            notificationId = (long)insertedId;
            return (affectedRecords > 0);
        }

        public List<VRNotification> GetVRNotifications(Guid notificationTypeId, VRNotificationParentTypes parentTypes, string eventKey, List<VRNotificationStatus> statusesToLoad)
        {
            string statusesString = statusesToLoad != null && statusesToLoad.Count > 0 ? String.Join(",", statusesToLoad.Select(itm => (int)itm)) : null;
            return GetItemsSP("VRNotification.sp_VRNotification_GetByNotificationType", VRNotificationMapper, notificationTypeId, parentTypes.ParentType1, parentTypes.ParentType2, eventKey, statusesString);
        }

        public void UpdateNotificationStatus(long notificationId, VRNotificationStatus vrNotificationStatus, long? executeBPInstanceId, long? clearBPInstanceId)
        {
            ExecuteNonQuerySP("[VRNotification].[sp_VRNotification_UpdateStatus]", notificationId, vrNotificationStatus, executeBPInstanceId, clearBPInstanceId);
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

            DateTime from = context.Query != null && context.Query.From.HasValue ? context.Query.From.Value : default(DateTime);
            DateTime to = context.Query != null && context.Query.To.HasValue ? context.Query.To.Value : default(DateTime);

            ExecuteReaderSP("[VRNotification].[sp_VRNotifications_GetFirstPage]", (reader) =>
            {
                while (reader.Read() && !isFinalRow)
                    isFinalRow = context.onItemReady(VRNotificationMapper(reader));

                if (reader.NextResult())
                    while (reader.Read())
                        context.MaxTimeStamp = GetReaderValue<byte[]>(reader, "MaxTimestamp");

            }, context.NotificationTypeId, context.NbOfRows, description, statusIds, alerttLevelIds, ToDBNullIfDefault(from), ToDBNullIfDefault(to));
        }

        public List<VRNotification> GetUpdateVRNotifications(IVRNotificationUpdateContext context)
        {
            string description = null;
            if (context.Query != null && !string.IsNullOrEmpty(context.Query.Description))
                description = context.Query.Description;

            string alerttLevelIds = null;
            if (context.Query != null && context.Query.AlertLevelIds != null && context.Query.AlertLevelIds.Count > 0)
                alerttLevelIds = string.Join<Guid>(",", context.Query.AlertLevelIds);

            DateTime from = context.Query != null && context.Query.From.HasValue ? context.Query.From.Value : default(DateTime);
            DateTime to = context.Query != null && context.Query.To.HasValue ? context.Query.To.Value : default(DateTime);

            List<VRNotification> notifications = new List<VRNotification>();
            ExecuteReaderSP("[VRNotification].[sp_VRNotifications_GetUpdated]", (reader) =>
            {
                while (reader.Read())
                    notifications.Add(VRNotificationMapper(reader));

                if (reader.NextResult())
                    while (reader.Read())
                        context.MaxTimeStamp = GetReaderValue<byte[]>(reader, "MaxTimestamp");

            }, context.NotificationTypeId, context.NbOfRows, context.MaxTimeStamp, description, alerttLevelIds, ToDBNullIfDefault(from), ToDBNullIfDefault(to));

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

            DateTime from = context.Query != null && context.Query.From.HasValue ? context.Query.From.Value : default(DateTime);
            DateTime to = context.Query != null && context.Query.To.HasValue ? context.Query.To.Value : default(DateTime);

            ExecuteReaderSP("[VRNotification].[sp_VRNotifications_GetBeforeID]", (reader) =>
            {
                while (reader.Read() && !isFinalRow)
                    isFinalRow = context.onItemReady(VRNotificationMapper(reader));

            }, context.NotificationTypeId, context.NbOfRows, context.LessThanID, description, statusIds, alerttLevelIds, ToDBNullIfDefault(from), ToDBNullIfDefault(to));
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
                ExecuteBPInstanceID = GetReaderValue<long?>(reader, "ExecuteBPInstanceID"),
                ClearBPInstanceID = GetReaderValue<long?>(reader, "ClearBPInstanceID"),
                ErrorMessage = reader["ErrorMessage"] as string,
                CreationTime = (DateTime)reader["CreationTime"],
                Data = reader["Data"] != DBNull.Value ? Serializer.Deserialize<VRNotificationData>(reader["Data"] as string) : null,
                EventPayload = reader["EventPayload"] != DBNull.Value ? Serializer.Deserialize<IVRActionEventPayload>(reader["EventPayload"] as string) : null
            };
        }

        #endregion

    }
}