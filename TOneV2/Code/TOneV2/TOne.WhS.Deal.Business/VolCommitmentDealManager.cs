using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using TOne.WhS.Deal.Entities;
using TOne.WhS.BusinessEntity.Business;
using System.ComponentModel;

namespace TOne.WhS.Deal.Business
{
    public class VolCommitmentDealManager : BaseDealManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<DealDefinitionDetail> GetFilteredVolCommitmentDeals(Vanrise.Entities.DataRetrievalInput<VolCommitmentDealQuery> input)
        {
            var cachedEntities = this.GetCachedVolCommitmentDeals();
            Func<DealDefinition, bool> filterExpression = (deal) =>
            {
                if (input.Query.Name != null && !deal.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (input.Query.Type.HasValue && input.Query.Type.Value != (deal.Settings as VolCommitmentDealSettings).DealType)
                    return false;
                if (input.Query.CarrierAccountIds != null && !input.Query.CarrierAccountIds.Contains((deal.Settings as VolCommitmentDealSettings).CarrierAccountId))
                    return false;
                return true;
            };

            ResultProcessingHandler<DealDefinitionDetail> handler = new ResultProcessingHandler<DealDefinitionDetail>()
            {
                ExportExcelHandler = new VolCommitmentDealExcelExportHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(VolCommitmentDealLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedEntities.ToBigResult(input, filterExpression, DealDeinitionDetailMapper), handler);
        }

        public override DealDefinitionDetail DealDeinitionDetailMapper(DealDefinition deal)
        {
            VolCommitmentDetail detail = new VolCommitmentDetail()
            {
                Entity = deal,
            };

            VolCommitmentDealSettings settings = deal.Settings as VolCommitmentDealSettings;
            int carrierAccountId = settings.CarrierAccountId;

            detail.CarrierAccountName = new CarrierAccountManager().GetCarrierAccountName(carrierAccountId);
            detail.TypeDescription = Utilities.GetEnumAttribute<VolCommitmentDealType, DescriptionAttribute>(settings.DealType).Description;
            detail.StatusDescription = Utilities.GetEnumAttribute<DealStatus, DescriptionAttribute>(settings.Status).Description;
            detail.IsEffective = settings.BeginDate <= DateTime.Now.Date && settings.EndDate >= DateTime.Now.Date;
            detail.CurrencySymbole = new CurrencyManager().GetCurrencySymbol(settings.CurrencyId);

            return detail;
        }

        public DealDefinition GetVolumeCommitmentHistoryDetailbyHistoryId(int volumeCommitmentHistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var dealDefinition = s_vrObjectTrackingManager.GetObjectDetailById(volumeCommitmentHistoryId);
            return dealDefinition.CastWithValidate<DealDefinition>("DealDefinition : historyId ", volumeCommitmentHistoryId);
        }

        public override BaseDealLoggableEntity GetLoggableEntity()
        {
            return VolCommitmentDealLoggableEntity.Instance;
        }

        public IEnumerable<DealDefinition> GetCachedVolCommitmentDeals()
        {
            return GetCachedDealsByConfigId().GetRecord(VolCommitmentDealSettings.VolCommitmentDealSettingsConfigId); ;
        }
        #endregion

        #region Private Classes
        private class VolCommitmentDealExcelExportHandler : ExcelExportHandler<DealDefinitionDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<DealDefinitionDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Volume Commitment",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Id" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Description", Width = 40 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Type" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Carrier", Width = 25 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Effective" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null && record.Entity.Settings != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            if (record.Entity != null && record.Entity.Settings != null)
                            {
                                var settings = (VolCommitmentDealSettings)record.Entity.Settings;
                                row.Cells.Add(new ExportExcelCell { Value = record.Entity.DealId });
                                row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
                                row.Cells.Add(new ExportExcelCell { Value = Vanrise.Common.Utilities.GetEnumDescription(settings.DealType) });
                                row.Cells.Add(new ExportExcelCell { Value = record.CarrierAccountName });
                                row.Cells.Add(new ExportExcelCell { Value = settings.BeginDate });
                                row.Cells.Add(new ExportExcelCell { Value = settings.EndDate });
                                row.Cells.Add(new ExportExcelCell { Value = record.IsEffective });
                            }
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        private class VolCommitmentDealLoggableEntity : BaseDealLoggableEntity
        {
            public static VolCommitmentDealLoggableEntity Instance = new VolCommitmentDealLoggableEntity();
            public VolCommitmentDealLoggableEntity()
            {

            }
            static VolCommitmentDealManager s_volCommitmentDealManager = new VolCommitmentDealManager();

            public override string EntityUniqueName
            {
                get { return "WhS_Deal_VolCommitmentDeal"; }
            }
            public override string EntityDisplayName
            {
                get { return "Vol Commitment Deal"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_Deal_VolCommitmentDeal_ViewHistoryItem"; }
            }
            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                DealDefinition dealDefinition = context.Object.CastWithValidate<DealDefinition>("context.Object");
                return s_volCommitmentDealManager.GetDealName(dealDefinition);
            }
        }
        #endregion

        #region IBusinessEntityManager
        public override List<dynamic> GetAllEntities(Vanrise.GenericData.Entities.IBusinessEntityGetAllContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetEntity(Vanrise.GenericData.Entities.IBusinessEntityGetByIdContext context)
        {
            throw new NotImplementedException();
        }

        public override string GetEntityDescription(Vanrise.GenericData.Entities.IBusinessEntityDescriptionContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetEntityId(Vanrise.GenericData.Entities.IBusinessEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(Vanrise.GenericData.Entities.IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetParentEntityId(Vanrise.GenericData.Entities.IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsCacheExpired(Vanrise.GenericData.Entities.IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        public override dynamic MapEntityToInfo(Vanrise.GenericData.Entities.IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}