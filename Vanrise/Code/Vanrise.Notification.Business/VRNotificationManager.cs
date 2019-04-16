using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
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
                    IsAutoClearable = input.IsAutoClearable
                },
                EventPayload = input.EventPayload
            };
            var notificationDataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            notificationDataManager.Insert(notification, out notificationId);
            var executeNotificationProcessInput = new ExecuteNotificationProcessInput
            {
                NotificationId = notificationId,
                NotificationTypeId = input.NotificationTypeId,
                EntityId = input.EntityId,
                EventKey = input.EventKey,
                ProcessTitle = input.Description != null ? input.Description : null,
                UserId = input.UserId,
                AlertRuleId = input.AlertRuleId
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
                RollbackEventPayload = input.RollbackEventPayload,
                NotificationTypeId = input.NotificationTypeId,
                EntityId = input.EntityId,
                ParentTypes = input.ParentTypes,
                ProcessTitle = input.Description,
                UserId = input.UserId,
                AlertRuleId = input.AlertRuleId
            };
            var createProcessInput = new Vanrise.BusinessProcess.Entities.CreateProcessInput
            {
                InputArguments = clearNotificationInput
            };
            _bpInstanceManager.CreateNewProcess(createProcessInput);
        }

        public Dictionary<string, VRNotification> GetNotClearedNotifications(Guid notificationTypeId, VRNotificationParentTypes parentTypes, List<string> eventKeys, DateTime? notificationCreatedAfter)
        {
            IVRNotificationDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            List<VRNotification> notClearedNotifications = dataManager.GetNotClearedNotifications(notificationTypeId, parentTypes, eventKeys, notificationCreatedAfter);
            VRAlertLevelManager vrAlertLevelManager = new VRAlertLevelManager();

            if (notClearedNotifications == null)
                return null;

            Dictionary<string, VRNotification> notifications = new Dictionary<string, VRNotification>();
            VRNotification matchedNotification;

            foreach (VRNotification notClearedNotification in notClearedNotifications)
            {
                if (notifications.TryGetValue(notClearedNotification.EventKey, out matchedNotification))
                {
                    if (vrAlertLevelManager.GetAlertLevelWeight(matchedNotification.AlertLevelId) < vrAlertLevelManager.GetAlertLevelWeight(notClearedNotification.AlertLevelId))
                        notifications[notClearedNotification.EventKey] = notClearedNotification;
                }
                else
                {
                    notifications.Add(notClearedNotification.EventKey, notClearedNotification);
                }
            }

            return notifications;
        }

        public void UpdateNotificationStatus(long notificationId, VRNotificationStatus vrNotificationStatus, IVRActionRollbackEventPayload rollbackEventPayload = null, long? executeBPInstanceId = null, long? clearBPInstanceId = null)
        {
            IVRNotificationDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            dataManager.UpdateNotificationStatus(notificationId, vrNotificationStatus, rollbackEventPayload, executeBPInstanceId, clearBPInstanceId);
        }

        public T GetVRNotificationEventPayload<T>(VRNotification vrNotification) where T : class, IVRActionEventPayload
        {
            vrNotification.ThrowIfNull<VRNotification>("vrNotification");
            return vrNotification.EventPayload.CastWithValidate<T>("vrNotification.EventPayload", vrNotification.VRNotificationId);
        }

        public ExcelResult ExportVRNotifications(VRNotificationExportInput input, out string fileName)
        {
            Guid notificationTypeId = input.NotificationTypeId;

            VRNotificationType vrNotificationType = new VRNotificationTypeManager().GetNotificationType(notificationTypeId);
            vrNotificationType.ThrowIfNull("vrNotificationType", notificationTypeId);

            fileName = vrNotificationType.Name;

            var vrNotificationTypeExtendedSettings = new VRNotificationTypeManager().GetVRNotificationTypeExtendedSettings(notificationTypeId);
            var headersByName = vrNotificationTypeExtendedSettings.GetNotificationFieldTitlesByName();

            if (headersByName == null || headersByName.Count == 0)
                return null;

            IVRNotificationDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            ExcelManager excelManager = new ExcelManager();

            var vrNotifications = dataManager.GetFilteredVRNotifications(input);
            return excelManager.ExportExcel<VRNotificationDetail>(GenerateExcelSheet(headersByName, vrNotifications, input.ExtendedQuery, vrNotificationTypeExtendedSettings));
        }

        public VRNotificationUpdateOutput GetFirstPageVRNotifications(VRNotificationFirstPageInput input)
        {
            List<VRNotification> vrNotifications = new List<VRNotification>();

            var vrNotificationTypeExtendedSettings = new VRNotificationTypeManager().GetVRNotificationTypeExtendedSettings(input.NotificationTypeId);

            Func<VRNotification, bool> onItemReady = (vrNotification) =>
            {
                bool isFinalRow = false;

                if (IsNotificationMatched(vrNotification, input.ExtendedQuery, vrNotificationTypeExtendedSettings))
                {
                    vrNotifications.Add(vrNotification);

                    if (vrNotifications.Count == input.NbOfRows)
                        isFinalRow = true;
                }

                return isFinalRow;
            };

            VRNotificationFirstPageContext vrNotificationFirstPageContext = new VRNotificationFirstPageContext()
            {
                NotificationTypeId = input.NotificationTypeId,
                NbOfRows = input.ExtendedQuery == null ? input.NbOfRows : long.MaxValue,
                Query = input.Query,
                onItemReady = onItemReady
            };
            IVRNotificationDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            dataManager.GetFirstPageVRNotifications(vrNotificationFirstPageContext);

            VRNotificationUpdateOutput vrNotificationUpdateOutput = new VRNotificationUpdateOutput();
            vrNotificationUpdateOutput.VRNotificationDetails = vrNotifications.Select(VRNotificationDetailMapper).ToList(); ;
            vrNotificationUpdateOutput.LastUpdateHandle = vrNotificationFirstPageContext.LastUpdateHandle;

            return vrNotificationUpdateOutput;
        }

        public VRNotificationUpdateOutput GetUpdatedVRNotifications(VRNotificationUpdateInput input)
        {
            var vrNotificationTypeExtendedSettings = new VRNotificationTypeManager().GetVRNotificationTypeExtendedSettings(input.NotificationTypeId);

            VRNotificationUpdateContext vrNotificationUpdateContext = new VRNotificationUpdateContext()
            {
                NotificationTypeId = input.NotificationTypeId,
                NbOfRows = input.ExtendedQuery == null ? input.NbOfRows : long.MaxValue,
                Query = input.Query,
                LastUpdateHandle = input.LastUpdateHandle
            };
            IVRNotificationDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            List<VRNotification> getUpdatedNotifications = dataManager.GetUpdateVRNotifications(vrNotificationUpdateContext);

            IEnumerable<VRNotification> matchedVRNotification = getUpdatedNotifications.FindAllRecords(item => IsNotificationMatched(item, input.ExtendedQuery, vrNotificationTypeExtendedSettings));

            VRNotificationUpdateOutput vrNotificationUpdateOutput = new VRNotificationUpdateOutput();
            vrNotificationUpdateOutput.VRNotificationDetails = matchedVRNotification != null ? matchedVRNotification.Select(VRNotificationDetailMapper).ToList() : null;
            vrNotificationUpdateOutput.LastUpdateHandle = vrNotificationUpdateContext.LastUpdateHandle;

            return vrNotificationUpdateOutput;
        }

        public List<VRNotificationDetail> GetBeforeIdVRNotifications(VRNotificationBeforeIdInput input)
        {
            List<VRNotification> vrNotifications = new List<VRNotification>();

            var vrNotificationTypeExtendedSettings = new VRNotificationTypeManager().GetVRNotificationTypeExtendedSettings(input.NotificationTypeId);

            Func<VRNotification, bool> onItemReady = (vrNotification) =>
            {
                bool isFinalRow = false;

                if (IsNotificationMatched(vrNotification, input.ExtendedQuery, vrNotificationTypeExtendedSettings))
                {
                    vrNotifications.Add(vrNotification);

                    if (vrNotifications.Count == input.NbOfRows)
                        isFinalRow = true;
                }

                return isFinalRow;
            };

            VRNotificationBeforeIdContext vrNotificationBeforeIdContext = new VRNotificationBeforeIdContext()
            {
                NotificationTypeId = input.NotificationTypeId,
                NbOfRows = input.ExtendedQuery == null ? input.NbOfRows : long.MaxValue,
                LessThanID = input.LessThanID,
                Query = input.Query,
                onItemReady = onItemReady
            };
            IVRNotificationDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            dataManager.GetBeforeIdVRNotifications(vrNotificationBeforeIdContext);

            return vrNotifications.Select(VRNotificationDetailMapper).ToList();
        }

        #endregion

        #region Mappers

        VRNotificationDetail VRNotificationDetailMapper(VRNotification vrNotification)
        {
            VRNotificationDetail vrNotificationDetail = new VRNotificationDetail();
            vrNotificationDetail.Entity = vrNotification;

            VRAlertLevel vrAlertLevel = new VRAlertLevelManager().GetAlertLevel(vrNotification.AlertLevelId);
            vrAlertLevel.ThrowIfNull<VRAlertLevel>("vrAlertLevel", vrNotification.AlertLevelId);
            vrAlertLevel.Settings.ThrowIfNull<VRAlertLevelSettings>("vrAlertLevel.Settings", vrNotification.AlertLevelId);
            vrNotificationDetail.AlertLevelDescription = vrAlertLevel.Name;

            StyleDefinition styleDefinition = new StyleDefinitionManager().GetStyleDefinition(vrAlertLevel.Settings.StyleDefinitionId);
            styleDefinition.ThrowIfNull<StyleDefinition>("styleDefinition", styleDefinition.StyleDefinitionId);
            styleDefinition.ThrowIfNull<StyleDefinition>("styleDefinition.StyleDefinitionSettings", styleDefinition.StyleDefinitionId);
            vrNotificationDetail.AlertLevelStyle = styleDefinition.StyleDefinitionSettings.StyleFormatingSettings;

            var vrNotificationTypeExtendedSettings = new VRNotificationTypeManager().GetVRNotificationTypeExtendedSettings(vrNotification.TypeId);
            vrNotificationDetail.VRNotificationEventPayload = vrNotificationTypeExtendedSettings.GetNotificationDetailEventPayload(new VRNotificationTypeGetNotificationEventPayloadContext { VRNotification = vrNotification });

            return vrNotificationDetail;
        }

        #endregion

        #region Private Methods 

        private ExportExcelSheet GenerateExcelSheet(Dictionary<string, string> fieldTitlesByName, List<VRNotification> vrNotifications, VRNotificationExtendedQuery vrNotificationExtendedQuery, VRNotificationTypeExtendedSettings vrNotificationTypeExtendedSettings)
        {
            ExportExcelSheet sheet = new ExportExcelSheet()
            {
                Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() },
                Rows = new List<ExportExcelRow>()
            };

            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Alert Level" });
            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Creation Time", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.DateTime });
            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Status" });
            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Description" });

            if (fieldTitlesByName == null || fieldTitlesByName.Count == 0)
                return sheet;

            List<string> headerNames = new List<string>();

            foreach (var fieldTitleKpv in fieldTitlesByName)
            {
                headerNames.Add(fieldTitleKpv.Key);

                var fieldTitle = fieldTitleKpv.Value;
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = fieldTitle });
            }

            if (vrNotifications == null || vrNotifications.Count == 0)
                return sheet;

            foreach (var notification in vrNotifications)
            {
                if (!IsNotificationMatched(notification, vrNotificationExtendedQuery, vrNotificationTypeExtendedSettings))
                    continue;

                var notificationDetail = VRNotificationDetailMapper(notification);

                var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                sheet.Rows.Add(row);

                row.Cells.Add(new ExportExcelCell { Value = notificationDetail.AlertLevelDescription });
                row.Cells.Add(new ExportExcelCell { Value = notificationDetail.Entity.CreationTime });
                row.Cells.Add(new ExportExcelCell { Value = notificationDetail.StatusDescription });
                row.Cells.Add(new ExportExcelCell { Value = notificationDetail.Entity.Description });

                Dictionary<string, object> fieldValuesByName = notificationDetail.VRNotificationEventPayload.GetNotificationFieldValuesByName();
                if (fieldValuesByName != null || fieldValuesByName.Count == 0)
                {
                    foreach (var header in headerNames)
                    {
                        object fieldValue = fieldValuesByName.GetRecord(header);
                        row.Cells.Add(new ExportExcelCell { Value = fieldValue });
                    }
                }
            }
            return sheet;
        }

        private bool IsNotificationMatched(VRNotification vrNotification, VRNotificationExtendedQuery vrNotificationExtendedQuery, VRNotificationTypeExtendedSettings vrNotificationTypeExtendedSettings)
        {
            var vrNotificationTypeIsMatchedContext = new VRNotificationTypeIsMatchedContext() { VRNotification = vrNotification, ExtendedQuery = vrNotificationExtendedQuery };
            return vrNotificationTypeExtendedSettings.IsVRNotificationMatched(vrNotificationTypeIsMatchedContext);
        }

        #endregion
    }
}
