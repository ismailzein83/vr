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
    public class VRActionAuditManager
    {
        static VRActionAuditLKUPManager s_lkupManager = new VRActionAuditLKUPManager();
        static IVRActionAuditDataManager s_dataManager = CommonDataManagerFactory.GetDataManager<IVRActionAuditDataManager>();

        #region Public Methods
        public void AuditAction(string url, string module, string entity, string action, string objectId, string objectName, string actionDescription, long? objectTrackingId)
        {
            int? userId;
            ContextFactory.GetContext().TryGetLoggedInUserId(out userId);
            int? urlId = null;
            if (url != null)
                urlId = s_lkupManager.GetLKUPId(VRActionAuditLKUPType.URL, url);
            int moduleId = s_lkupManager.GetLKUPId(VRActionAuditLKUPType.Module, module);
            int entityId = s_lkupManager.GetLKUPId(VRActionAuditLKUPType.Entity, entity);
            int actionId = s_lkupManager.GetLKUPId(VRActionAuditLKUPType.Action, action);
            s_dataManager.Insert(userId, urlId, moduleId, entityId, actionId, objectId, objectName, objectTrackingId, actionDescription);
        }
        public Vanrise.Entities.IDataRetrievalResult<VRActionAuditDetail> GetFilteredActionAudits(Vanrise.Entities.DataRetrievalInput<VRActionAuditQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new ActionAuditRequestHandler());

        }
        #endregion

        #region private Class
        private class ActionAuditRequestHandler : BigDataRequestHandler<VRActionAuditQuery, VRActionAudit, VRActionAuditDetail>
        {
            public override VRActionAuditDetail EntityDetailMapper(VRActionAudit entity)
            {
                VRActionAuditManager manager = new VRActionAuditManager();
                return manager.VRActionAuditDetailMapper(entity);
            }

            public override IEnumerable<VRActionAudit> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<VRActionAuditQuery> input)
            {
                var maxTop = new ConfigManager().GetMaxSearchRecordCount();
                if (input.Query.TopRecord > maxTop)
                    throw new VRBusinessException(string.Format("Top record count cannot be greater than {0}", maxTop));

                IVRActionAuditDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRActionAuditDataManager>();
                return dataManager.GetFilterdActionAudits(input.Query);
            }

            protected override ResultProcessingHandler<VRActionAuditDetail> GetResultProcessingHandler(DataRetrievalInput<VRActionAuditQuery> input, BigResult<VRActionAuditDetail> bigResult)
            {
                return new ResultProcessingHandler<VRActionAuditDetail>
                {
                    ExportExcelHandler = new VRActionAuditExcelExportHandler(input.Query)
                };
            }
        }
        private class VRActionAuditExcelExportHandler : ExcelExportHandler<VRActionAuditDetail>
        {
            VRActionAuditQuery _query;
            public VRActionAuditExcelExportHandler(VRActionAuditQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<VRActionAuditDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Action Audit",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Time", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "User" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Module"});
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Entity", Width = 30});
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Action" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Entity Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Notes", Width = 120 });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.VRActionAuditId });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.LogTime });
                            row.Cells.Add(new ExportExcelCell { Value = record.UserName });
                            row.Cells.Add(new ExportExcelCell { Value = record.ModuleName });
                            row.Cells.Add(new ExportExcelCell { Value = record.EntityName });
                            row.Cells.Add(new ExportExcelCell { Value = record.ActionName });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.ObjectName });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.ActionDescription });
                        }
                    }
                }

                context.MainSheet = sheet;
            }
        }
        #endregion

        #region private Method

        private VRActionAuditDetail VRActionAuditDetailMapper(VRActionAudit ActionAudit)
        {
            VRActionAuditLKUPManager VRLKUPM = new VRActionAuditLKUPManager();
            VRActionAuditDetail ActionAuditDetail = new VRActionAuditDetail();
            ActionAuditDetail.Entity = ActionAudit;
            if (ActionAudit.UserId.HasValue)
                ActionAuditDetail.UserName = BEManagerFactory.GetManager<IUserManager>().GetUserName(ActionAudit.UserId.Value);
            ActionAuditDetail.ModuleName = VRLKUPM.GetVRActionAuditLKUPName(ActionAudit.ModuleId);
            ActionAuditDetail.EntityName = VRLKUPM.GetVRActionAuditLKUPName(ActionAudit.EntityId);
            ActionAuditDetail.ActionName = VRLKUPM.GetVRActionAuditLKUPName(ActionAudit.ActionId);
            ActionAuditDetail.URLName = VRLKUPM.GetVRActionAuditLKUPName(ActionAudit.UrlId);

            return ActionAuditDetail;
        }
        #endregion
    }
}
