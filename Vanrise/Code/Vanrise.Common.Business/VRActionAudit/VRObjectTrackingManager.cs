using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.Security.Entities;


namespace Vanrise.Common.Business
{
    public class VRObjectTrackingManager
    {
        static IVRObjectTrackingDataManager s_dataManager = CommonDataManagerFactory.GetDataManager<IVRObjectTrackingDataManager>();
        static VRLoggableEntityManager s_loggableEntityManager = new VRLoggableEntityManager();
        static VRActionAuditLKUPManager s_actionAuditLKUPManager = new VRActionAuditLKUPManager();

        internal long TrackObjectAction(VRLoggableEntityBase loggableEntity, string objectId, Object obj, string action, string actionDescription, Object technicalInformation, VRActionAuditChangeInfo vrActionAuditChangeInfo)
        {
            int userId = ContextFactory.GetContext().GetLoggedInUserId();
            Guid loggableEntityId = s_loggableEntityManager.GetLoggableEntityId(loggableEntity);
            int actionId = s_actionAuditLKUPManager.GetLKUPId(VRActionAuditLKUPType.Action, action);
            return s_dataManager.Insert(userId, loggableEntityId, objectId, obj, actionId, actionDescription, technicalInformation, vrActionAuditChangeInfo);
        }

        public IDataRetrievalResult<VRObjectTrackingMetaDataDetail> GetFilteredObjectTracking(Vanrise.Entities.DataRetrievalInput<VRLoggableEntityQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new VRObjectTrackingHandler());

        }

        public VRLoggableEntitySettings GetVRLoggableEntitySettings(string uniqueName)
        {
            var logEntity = s_loggableEntityManager.GetLoggableEntity(uniqueName);
            return (logEntity != null) ? logEntity.Settings : null;
        }

        public object GetObjectDetailById(int VRObjectTrackingId)
        {
            IVRObjectTrackingDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRObjectTrackingDataManager>();
            return dataManager.GetObjectDetailById(VRObjectTrackingId);

        }

        private class VRObjectTrackingHandler : BigDataRequestHandler<VRLoggableEntityQuery, VRObjectTrackingMetaData, VRObjectTrackingMetaDataDetail>
        {
            public override VRObjectTrackingMetaDataDetail EntityDetailMapper(VRObjectTrackingMetaData entity)
            {
                VRObjectTrackingManager manager = new VRObjectTrackingManager();
                return manager.VRObjectTrackingDetailMapper(entity);
            }

            public override IEnumerable<VRObjectTrackingMetaData> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<VRLoggableEntityQuery> input)
            {
                IVRObjectTrackingDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRObjectTrackingDataManager>();
                VRLoggableEntityManager manager = new VRLoggableEntityManager();
                Guid loggableEntityId = manager.GetLoggableEntityId(input.Query.EntityUniqueName);
                return dataManager.GetAll(loggableEntityId, input.Query.ObjectId);
            }

            protected override ResultProcessingHandler<VRObjectTrackingMetaDataDetail> GetResultProcessingHandler(DataRetrievalInput<VRLoggableEntityQuery> input, BigResult<VRObjectTrackingMetaDataDetail> bigResult)
            {
                return new ResultProcessingHandler<VRObjectTrackingMetaDataDetail>
                {
                    ExportExcelHandler = new VRObjectTrackingExcelExportHandler()
                };
            }
        }


        private class VRObjectTrackingExcelExportHandler : ExcelExportHandler<VRObjectTrackingMetaDataDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<VRObjectTrackingMetaDataDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "History",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Time", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "User name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Action name" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Time });
                            row.Cells.Add(new ExportExcelCell { Value = record.UserName });
                            row.Cells.Add(new ExportExcelCell { Value = record.ActionName });
                        }
                    }
                }

                context.MainSheet = sheet;
            }
        }


        private VRObjectTrackingMetaDataDetail VRObjectTrackingDetailMapper(VRObjectTrackingMetaData objectTrackingMetaData)
        {
            VRActionAuditLKUPManager vrLKUPM = new VRActionAuditLKUPManager();
            VRObjectTrackingMetaDataDetail objectTrackingMetaDataDetail = new VRObjectTrackingMetaDataDetail();
            objectTrackingMetaDataDetail.Entity = objectTrackingMetaData;
            objectTrackingMetaDataDetail.UserName = BEManagerFactory.GetManager<IUserManager>().GetUserName(objectTrackingMetaData.UserId);
            objectTrackingMetaDataDetail.ActionName = vrLKUPM.GetVRActionAuditLKUPName(objectTrackingMetaData.ActionId);



            return objectTrackingMetaDataDetail;
        }

    }
}
