using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Notification.BP.Arguments;
using Vanrise.Notification.Data;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Business
{
    public class VRNotificationManager
    {
        #region Ctor/Properties

        Vanrise.BusinessProcess.Business.BPInstanceManager _bpInstanceManager = new Vanrise.BusinessProcess.Business.BPInstanceManager();

        #endregion

        #region Public Methods

        public CreateVRNotificationOutput CreateNotification(CreateVRNotificationInput input)
        {
            long notificationId = 0;
            var notification = new VRNotification
            {

                UserId = input.UserId,
                TypeId = input.NotificationTypeId,
                ParentTypes = input.ParentTypes,
                EventKey = input.EventKey,
                Status = VRNotificationStatus.New,
                AlertLevelId = input.AlertLevelId,
                Description = input.Description,
                Data = new VRNotificationData
                {
                    Actions = input.Actions,
                    ClearanceActions = input.ClearanceActions,
                    EventPayload = input.EventPayload,
                    IsAutoClearable = input.IsAutoClearable
                }
            };
            var notificationDataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            notificationDataManager.Insert(notification, out notificationId);
            var executeNotificationProcessInput = new ExecuteNotificationProcessInput
            {
                NotificationId = notificationId,
                ProcessTitle = input.Description != null ? input.Description : null,
                UserId = input.UserId
            };
            var createProcessInput = new Vanrise.BusinessProcess.Entities.CreateProcessInput
            {
                InputArguments = executeNotificationProcessInput
            };
            var createProcessOutput = _bpInstanceManager.CreateNewProcess(createProcessInput);
            return new CreateVRNotificationOutput
            {

            };
        }

        public VRNotification GetVRNotificationById(long vrNotificationId)
        {
            IVRNotificationDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            return dataManager.GetVRNotificationById(vrNotificationId);
        }

        public void ClearNotifications(ClearVRNotificationInput input)
        {
            ClearNotificationInput clearNotificationInput = new ClearNotificationInput
            {
                EventKey = input.EventKey,
                NotificationTypeId = input.NotificationTypeId,
                ParentTypes = input.ParentTypes,
                ProcessTitle = input.Description,
                UserId = input.UserId
            };
            var createProcessInput = new Vanrise.BusinessProcess.Entities.CreateProcessInput
            {
                InputArguments = clearNotificationInput
            };
            _bpInstanceManager.CreateNewProcess(createProcessInput);
        }

        public List<string> GetNotClearedNotificationsEventKeys(Guid notificationTypeId, VRNotificationParentTypes parentTypes, DateTime? notificationCreatedAfter)
        {
            throw new NotImplementedException();
        }

        public void UpdateNotificationStatus(long notificationId, VRNotificationStatus vrNotificationStatus)
        {
            IVRNotificationDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            dataManager.UpdateNotificationStatus(notificationId, vrNotificationStatus);
        }

        public VRNotificationUpdateOutput GetFirstPageVRNotifications(VRNotificationFirstPageInput input)
        {
            List<VRNotification> vrNotifications = new List<VRNotification>();
            byte[] maxTimestamp = null;

            long? nbOfRows = null;
            if (input.ExtendedQuery == null)
                nbOfRows = input.NbOfRows;

            var vrNotificationTypeExtendedSettings = new VRNotificationTypeManager().GetVRNotificationTypeExtendedSettings(input.NotificationTypeId);

            Func<VRNotification, bool> filterExpression = (vrNotification) =>
            {
                var context = new VRNotificationTypeIsMatchedContext() { VRNotification = vrNotification, ExtendedQuery = input.ExtendedQuery };
                return vrNotificationTypeExtendedSettings.IsVRNotificationMatched(context);
            };

            Func<VRNotification, bool> onItemReady = (vrNotification) =>
            {
                bool isFinalRow = false;

                if (filterExpression(vrNotification))
                {
                    vrNotifications.Add(vrNotification);

                    if (vrNotifications.Count == input.NbOfRows)
                        isFinalRow = true;
                }

                return isFinalRow;
            };

            IVRNotificationDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            dataManager.GetFirstPageVRNotifications(input, nbOfRows, ref maxTimestamp, onItemReady);

            VRNotificationUpdateOutput vrNotificationUpdateOutput = new VRNotificationUpdateOutput();
            vrNotificationUpdateOutput.VRNotificationDetails = vrNotifications.Select(VRNotificationDetailMapper).ToList(); ;
            vrNotificationUpdateOutput.MaxTimeStamp = maxTimestamp;

            return vrNotificationUpdateOutput;
        }

        public VRNotificationUpdateOutput GetUpdatedVRNotifications(VRNotificationUpdateInput input)
        {
            List<VRNotification> vrNotifications = new List<VRNotification>();
            byte[] maxTimestamp = null;

            var vrNotificationTypeExtendedSettings = new VRNotificationTypeManager().GetVRNotificationTypeExtendedSettings(input.NotificationTypeId);

            Func<VRNotification, bool> filterExpression = (vrNotification) =>
                {
                    var context = new VRNotificationTypeIsMatchedContext() { VRNotification = vrNotification, ExtendedQuery = input.ExtendedQuery };
                    return vrNotificationTypeExtendedSettings.IsVRNotificationMatched(context);
                };

            Action<VRNotification> onItemReady = (vrNotification) =>
                {
                    if (filterExpression(vrNotification))
                        vrNotifications.Add(vrNotification);
                };

            IVRNotificationDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            dataManager.GetUpdateVRNotifications(input, ref maxTimestamp, onItemReady);

            VRNotificationUpdateOutput vrNotificationUpdateOutput = new VRNotificationUpdateOutput();
            vrNotificationUpdateOutput.VRNotificationDetails = vrNotifications.Select(VRNotificationDetailMapper).ToList(); ;
            vrNotificationUpdateOutput.MaxTimeStamp = maxTimestamp;

            return vrNotificationUpdateOutput;
        }

        public List<VRNotificationDetail> GetBeforeIdVRNotifications(VRNotificationBeforeIdInput input)
        {
            List<VRNotification> vrNotifications = new List<VRNotification>();

            long? nbOfRows = null;
            if (input.ExtendedQuery == null)
                nbOfRows = input.NbOfRows;

            var vrNotificationTypeExtendedSettings = new VRNotificationTypeManager().GetVRNotificationTypeExtendedSettings(input.NotificationTypeId);

            Func<VRNotification, bool> filterExpression = (vrNotification) =>
                {
                    var context = new VRNotificationTypeIsMatchedContext() { VRNotification = vrNotification, ExtendedQuery = input.ExtendedQuery };
                    return vrNotificationTypeExtendedSettings.IsVRNotificationMatched(context);
                };

            Func<VRNotification, bool> onItemReady = (vrNotification) =>
                {
                    bool isFinalRow = false;

                    if (filterExpression(vrNotification))
                    {
                        vrNotifications.Add(vrNotification);

                        if (vrNotifications.Count == input.NbOfRows)
                            isFinalRow = true;
                    }

                    return isFinalRow;
                };

            IVRNotificationDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            dataManager.GetBeforeIdVRNotifications(input, nbOfRows, onItemReady);

            return vrNotifications.Select(VRNotificationDetailMapper).ToList();
        }

        #endregion

        #region Mappers

        VRNotificationDetail VRNotificationDetailMapper(VRNotification vrNotification)
        {
            var vrNotificationTypeExtendedSettings = new VRNotificationTypeManager().GetVRNotificationTypeExtendedSettings(vrNotification.TypeId);
            return vrNotificationTypeExtendedSettings.MapToNotificationDetail(new VRNotificationTypeMapToDetailContext { VRNotification = vrNotification });
        }

        #endregion
    }
}
