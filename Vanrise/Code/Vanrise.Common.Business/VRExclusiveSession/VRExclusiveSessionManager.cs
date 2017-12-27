using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRExclusiveSessionManager
    {
        static IVRExclusiveSessionDataManager s_dataManager = CommonDataManagerFactory.GetDataManager<IVRExclusiveSessionDataManager>();
        static Vanrise.Security.Entities.IUserManager s_userManager = Vanrise.Security.Entities.BEManagerFactory.GetManager<Vanrise.Security.Entities.IUserManager>();
        VRComponentTypeManager _vrComponentTypeManager;
        #region Public Methods

        public VRExclusiveSessionManager()
        {
            _vrComponentTypeManager = new VRComponentTypeManager();
        }
        public IDataRetrievalResult<VRExclusiveSessionDetail> GetFilteredVRExclusiveSessions(DataRetrievalInput<VRExclusiveSessionQuery> input)
        {
            var allVRExclusiveSessions = s_dataManager.GetAllVRExclusiveSessions(GetTimeOutInSeconds(), input.Query.SessionTypeIds).MapRecords(VRExclusiveSessionDetailMapper);
            Func<VRExclusiveSessionDetail, bool> filterExpression = (x) =>
            {
                if (input.Query.TargetName != null && !x.TargetName.ToLower().Contains(input.Query.TargetName.ToLower()))
                    return false;
                return true;
            };

            ResultProcessingHandler<VRExclusiveSessionDetail> handler = new ResultProcessingHandler<VRExclusiveSessionDetail>()
            {
                ExportExcelHandler = new VRExclusiveSessionDetailExcelExportHandler()
            };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRExclusiveSessions.ToBigResult(input, filterExpression), handler);
        }


        public VRExclusiveSessionTryTakeOutput TryTakeSession(VRExclusiveSessionTryTakeInput input)
        {
            s_dataManager.InsertIfNotExists(input.SessionTypeId, input.TargetId);
            string failureMessage;
            if (TryTakeSession(input.SessionTypeId, input.TargetId, out failureMessage))
            {
                return new VRExclusiveSessionTryTakeOutput
                {
                    IsSucceeded = true
                };
            }
            else
            {
                return new VRExclusiveSessionTryTakeOutput
                {
                    IsSucceeded = false,
                    FailureMessage = failureMessage
                };
            }
        }


        public IEnumerable<VRExclusiveSessionTypeInfo> GetVRExclusiveSessionTypeInfos(VRExclusiveSessionTypeInfoFilter filter)
        {
            return _vrComponentTypeManager.GetComponentTypes<VRExclusiveSessionTypeSettings, VRExclusiveSessionType>().MapRecords(VRExclusiveSessionTypeInfoMapper);
        }
        public VRExclusiveSessionTryKeepOutput TryKeepSession(VRExclusiveSessionTryKeepInput input)
        {
            string failureMessage;
            if (TryKeepSession(input.SessionTypeId, input.TargetId, out failureMessage))
            {
                return new VRExclusiveSessionTryKeepOutput
                {
                    IsSucceeded = true
                };
            }
            else
            {
                return new VRExclusiveSessionTryKeepOutput
                {
                    IsSucceeded = false,
                    FailureMessage = failureMessage
                };
            }
        }

        public void ReleaseSession(VRExclusiveSessionReleaseInput input)
        {
            if (input == null) return;
            int currentUserId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            s_dataManager.ReleaseSession(input.SessionTypeId, input.TargetId, currentUserId);
        }

        public void ForceReleaseSession(int vrExclusiveSessionId)
        {
            s_dataManager.ForceReleaseSession(vrExclusiveSessionId);
        }

        public void ForceReleaseAllSessions()
        {
            s_dataManager.ForceReleaseAllSessions();
        }

        #endregion

        #region Private Methods

        private bool TryTakeSession(Guid sessionTypeId, string targetId, out string failureMessage)
        {
            int currentUserId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            int takenByUserId;
            s_dataManager.TryTakeSession(sessionTypeId, targetId, currentUserId, GetTimeOutInSeconds(), out takenByUserId);
            if (currentUserId == takenByUserId)
            {
                failureMessage = null;
                return true;
            }
            else
            {
                failureMessage = String.Format("Session is locked by '{0}'", s_userManager.GetUserName(takenByUserId));
                return false;
            }
        }

        private bool TryKeepSession(Guid sessionTypeId, string targetId, out string failureMessage)
        {
            int currentUserId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            int takenByUserId;
            s_dataManager.TryKeepSession(sessionTypeId, targetId, currentUserId, GetTimeOutInSeconds(), out takenByUserId);
            if (currentUserId == takenByUserId)
            {
                failureMessage = null;
                return true;
            }
            else
            {
                failureMessage = String.Format("Session is locked by '{0}'", s_userManager.GetUserName(takenByUserId));
                return false;
            }
        }

        public bool DoesUserHaveTakeAccess(Guid sessionTypeId)
        {
            var extendedTypeSettings = this.GetVRExclusiveSessionTypeExtendedSettingsById(sessionTypeId);
            var context = new VRExclusiveSessionDoesUserHaveTakeAccessContext { UserId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId() };
            extendedTypeSettings.ThrowIfNull("DoesUserHaveTakeAccess");
            return extendedTypeSettings.DoesUserHaveTakeAccess(context);

        }

        private int GetTimeOutInSeconds()
        {
            var configManager = new ConfigManager();
            return configManager.GetSessionLockTimeOutInSeconds();
        }

        public IEnumerable<VRExclusiveSessionTypeExtendedSettingsConfig> GetVRExclusiveSessionTypeExtendedSettingsConfigs()
        {
            var configManager = new ExtensionConfigurationManager();
            return configManager.GetExtensionConfigurations<VRExclusiveSessionTypeExtendedSettingsConfig>(VRExclusiveSessionTypeExtendedSettingsConfig.EXTENSION_TYPE);
        }

        private class VRExclusiveSessionDoesUserHaveTakeAccessContext : IVRExclusiveSessionDoesUserHaveTakeAccessContext
        {
            public int UserId
            {
                get;
                set;
            }
        }
        private VRExclusiveSessionTypeInfo VRExclusiveSessionTypeInfoMapper(VRExclusiveSessionType vrSessionType)
        {
            return new VRExclusiveSessionTypeInfo
            {
                VRExclusiveSessionTypeId = vrSessionType.VRComponentTypeId,
                Name = vrSessionType.Name,
            };
        }

        private VRExclusiveSessionDetail VRExclusiveSessionDetailMapper(VRExclusiveSession vrExclusiveSession)
        {
            var targetNameContext = new VRExclusiveSessionGetTargetNameContext() { TargetId = vrExclusiveSession.TargetId };
            var sessionType = _vrComponentTypeManager.GetComponentType(vrExclusiveSession.SessionTypeId);
            var sessionTypeSetting = sessionType.Settings as VRExclusiveSessionTypeSettings;



            return new VRExclusiveSessionDetail
            {
                VRExclusiveSessionId = vrExclusiveSession.VRExclusiveSessionID,
                SessionTypeId = vrExclusiveSession.SessionTypeId,
                SessionType = sessionType != null ? sessionType.Name : null,
                TargetId = vrExclusiveSession.TargetId,
                TargetName = sessionTypeSetting != null && sessionTypeSetting.ExtendedSettings != null ? sessionTypeSetting.ExtendedSettings.GetTargetName(targetNameContext) : null,
                TakenByUserId = vrExclusiveSession.TakenByUserId,
                LockedByUser = s_userManager.GetUserName(vrExclusiveSession.TakenByUserId),
                LastTakenUpdateTime = vrExclusiveSession.LastTakenUpdateTime,
                CreatedTime = vrExclusiveSession.CreatedTime,
                TakenTime = vrExclusiveSession.TakenTime

            };
        }

        private class VRExclusiveSessionDetailExcelExportHandler : ExcelExportHandler<VRExclusiveSessionDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<VRExclusiveSessionDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Exclusive Sessions",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Id", Width = 50 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Session Type", Width = 50 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Name", Width = 50 }); // target name
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Locked By User" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Taken Time", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.DateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Created Time", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.DateTime });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.VRExclusiveSessionId });
                            row.Cells.Add(new ExportExcelCell { Value = record.SessionType });
                            row.Cells.Add(new ExportExcelCell { Value = record.TargetName });
                            row.Cells.Add(new ExportExcelCell { Value = record.LockedByUser });
                            row.Cells.Add(new ExportExcelCell { Value = record.TakenTime });
                            row.Cells.Add(new ExportExcelCell { Value = record.CreatedTime });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        #endregion

        public VRExclusiveSessionTypeExtendedSettings GetVRExclusiveSessionTypeExtendedSettingsById(Guid configId)
        {
            VRComponentTypeManager vrComponentTypeManager = new VRComponentTypeManager();
            var sessionTypeSettings = vrComponentTypeManager.GetComponentTypeSettings<VRExclusiveSessionTypeSettings>(configId);
            sessionTypeSettings.ThrowIfNull("GetVRExclusiveSessionTypeExtendedSettingsById");
            return sessionTypeSettings.ExtendedSettings;
        }
    }
}
