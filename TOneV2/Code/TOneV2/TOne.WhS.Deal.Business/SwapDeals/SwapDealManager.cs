using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using TOne.WhS.Deal.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Deal.Entities.Settings;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Data;
using Vanrise.Caching;

namespace TOne.WhS.Deal.Business
{
    public class SwapDealManager : BaseDealManager
    {
        #region Public Methods

        public SwapDealSettingsDetail GetSwapDealSettingsDetail(int dealId)
        {
            SwapDealSettings swapDealSettings = GetDealSettings<SwapDealSettings>(dealId);

            CarrierAccount carrierAccount = new CarrierAccountManager().GetCarrierAccount(swapDealSettings.CarrierAccountId);
            carrierAccount.ThrowIfNull("carrierAccount", swapDealSettings.CarrierAccountId);

            int? sellingNumberPlan = carrierAccount.SellingNumberPlanId;
            if (!sellingNumberPlan.HasValue)
                throw new NullReferenceException(string.Format("sellingNumberPlan. CarrierAccountId: '{0}'", swapDealSettings.CarrierAccountId));

            List<long> saleZoneIds = swapDealSettings.Inbounds.SelectMany(itm => itm.SaleZones.Select(z => z.ZoneId)).ToList();
            List<long> supplierZoneIds = swapDealSettings.Outbounds.SelectMany(itm => itm.SupplierZones.Select(z => z.ZoneId)).ToList();

            return new SwapDealSettingsDetail()
            {
                SwapDealId = dealId,
                CarrierAccountId = swapDealSettings.CarrierAccountId,
                SellingNumberPlanId = sellingNumberPlan.Value,
                SaleZoneIds = saleZoneIds,
                SupplierZoneIds = supplierZoneIds,
                BED = swapDealSettings.BeginDate,
                EED = swapDealSettings.EndDate
            };
        }

        public IEnumerable<DealDefinition> GetSwapDealsEffectiveAfterDate(DateTime effectiveAfter)
        {
            List<DealDefinition> dealDefinitions = new List<DealDefinition>();
            var deals = GetCachedSwapDeals();
            foreach (var dealDefinition in deals)
            {
                DateTime? dealEED = GetDealEED(dealDefinition.Settings);
                if (!dealEED.HasValue || dealEED >= effectiveAfter)
                    dealDefinitions.Add(dealDefinition);
            }
            return dealDefinitions;
        }
        public IEnumerable<DealDefinition> GetSwapDealsBetweenDate(DateTime beginDate, DateTime endDate)
        {
            var dealDefinitions = new List<DealDefinition>();
            var deals = GetCachedSwapDeals();

            if (deals == null)
                return null;

            foreach (var dealDefinition in deals)
            {
                DateTime? dealEED = GetDealEED(dealDefinition.Settings);
                if (dealDefinition.Settings.BeginDate >= beginDate && dealDefinition.Settings.EndDate < dealEED)
                    dealDefinitions.Add(dealDefinition);
            }
            return dealDefinitions;
        }
        public Vanrise.Entities.IDataRetrievalResult<DealDefinitionDetail> GetFilteredSwapDeals(Vanrise.Entities.DataRetrievalInput<SwapDealQuery> input)
        {
            var cachedEntities = this.GetCachedSwapDeals();
            Func<DealDefinition, bool> filterExpression = (deal) =>
            {
                if (input.Query.Name != null && !deal.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (input.Query.CarrierAccountIds != null && !input.Query.CarrierAccountIds.Contains((deal.Settings as SwapDealSettings).CarrierAccountId))
                    return false;
                if (input.Query.Status != null && !input.Query.Status.Contains(deal.Settings.Status))
                    return false;
                return true;
            };

            ResultProcessingHandler<DealDefinitionDetail> handler = new ResultProcessingHandler<DealDefinitionDetail>()
            {
                ExportExcelHandler = new SwapDealExcelExportHandler()
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedEntities.ToBigResult(input, filterExpression, DealDeinitionDetailMapper), handler);
        }

        public DealDefinition GetSwapDealHistoryDetailbyHistoryId(int swapDealHistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var dealDefinition = s_vrObjectTrackingManager.GetObjectDetailById(swapDealHistoryId);
            return dealDefinition.CastWithValidate<DealDefinition>("DealDefinition : historyId ", swapDealHistoryId);
        }

        public SwapDealSettingData GetSwapDealSettingData()
        {
            var settingManager = new SettingManager();
            Setting setting = settingManager.GetSettingByType(Constants.SwapDealSettings);
            if (setting == null)
                throw new NullReferenceException("setting");
            if (setting.Data == null)
                throw new NullReferenceException("setting.Data");
            var swapDealAnalysisSettingData = setting.Data as SwapDealSettingData;
            if (swapDealAnalysisSettingData == null)
                throw new NullReferenceException("swapDealAnalysisSettingData");
            return swapDealAnalysisSettingData;
        }

        public InsertDealOperationOutput<List<DealDefinitionDetail>> ReoccurDeal(int dealId, int reoccuringNumber, ReoccuringType reoccuringType)
        {
            InsertDealOperationOutput<List<DealDefinitionDetail>> insertOperationOutput = new InsertDealOperationOutput<List<DealDefinitionDetail>>
            {
                Result = Vanrise.Entities.InsertOperationResult.Failed,
                InsertedObject = null
            };
            var deal = GetDeal(dealId);

            if (deal == null)
                throw new NullReferenceException(dealId.ToString());

            if (!deal.Settings.IsReoccurrable)
                return insertOperationOutput;

            var dealDefinitionManager = new DealDefinitionManager();

            //Getting Reoccured Deals
            List<DealDefinition> reoccuredDeals = dealDefinitionManager.GetReoccurredDeals(deal, reoccuringNumber, reoccuringType);
            if (!reoccuredDeals.Any())
                return insertOperationOutput;

            //Validation
            insertOperationOutput.ValidationMessages = new List<string>();
            var errorMessages = dealDefinitionManager.ValidatingDeals(reoccuredDeals);
            if (errorMessages.Any())
            {
                insertOperationOutput.ValidationMessages.AddRange(errorMessages);
                return insertOperationOutput;
            }

            //Inserting
            IDealDataManager dataManager = DealDataManagerFactory.GetDataManager<IDealDataManager>();
            deal.Settings.IsReoccurrable = false;
            if (dataManager.Update(deal))
            {
                errorMessages = new List<string>();
                insertOperationOutput.InsertedObject = new List<DealDefinitionDetail>();
                foreach (var reoccuredDeal in reoccuredDeals)
                {
                    int insertedId = -1;
                    if (dataManager.Insert(reoccuredDeal, out insertedId))
                    {
                        CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                        insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                        insertOperationOutput.InsertedObject.Add(DealDeinitionDetailMapper(reoccuredDeal));
                        reoccuredDeal.DealId = insertedId;
                    }
                    else
                    {
                        errorMessages.Add(string.Format("Deal Already exist {0}", reoccuredDeal.Name));
                    }
                }
            }
            if (errorMessages.Any())
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
                insertOperationOutput.ValidationMessages.AddRange(errorMessages);
            }

            return insertOperationOutput;
        }

        public override DealDefinitionDetail DealDeinitionDetailMapper(DealDefinition deal)
        {
            SwapDealDetail detail = new SwapDealDetail()
            {
                Entity = deal,
            };

            SwapDealSettings settings = deal.Settings as SwapDealSettings;
            int carrierAccountId = settings.CarrierAccountId;

            detail.CarrierAccountName = new CarrierAccountManager().GetCarrierAccountName(carrierAccountId);
            detail.TypeDescription = Utilities.GetEnumAttribute<DealType, DescriptionAttribute>(settings.DealType).Description;
            detail.StatusDescription = Utilities.GetEnumAttribute<DealStatus, DescriptionAttribute>(settings.Status).Description;
            detail.ContractDescription = Utilities.GetEnumAttribute<DealContract, DescriptionAttribute>(settings.DealContract).Description;
            detail.IsEffective = settings.BeginDate <= DateTime.Now.Date && settings.EndDate >= DateTime.Now.Date;
            detail.BuyingAmount = settings.Outbounds.Sum(x => x.Volume * x.Rate);
            detail.BuyingVolume = settings.Outbounds.Sum(x => x.Volume);
            detail.SellingAmount = settings.Inbounds.Sum(x => x.Volume * x.Rate);
            detail.SellingVolume = settings.Inbounds.Sum(x => x.Volume);
            detail.CurrencySymbole = new CurrencyManager().GetCurrencySymbol(settings.CurrencyId);

            return detail;
        }


        public override BaseDealLoggableEntity GetLoggableEntity()
        {
            return SwapDealLoggableEntity.Instance;
        }

        protected IEnumerable<DealDefinition> GetCachedSwapDeals()
        {
            return GetCachedDealsByConfigId().GetRecord(SwapDealSettings.SwapDealSettingsConfigId);
        }

        protected DealDefinition GetDeal(int dealId)
        {
            var deals = GetCachedSwapDeals();
            if (deals != null && deals.Any())
                return deals.FirstOrDefault(d => d.DealId == dealId);
            return null;
        }
        #endregion

        #region private Methods

        private DateTime? GetDealEED(DealSettings dealSetting)
        {
            var swapDealSetting = dealSetting as SwapDealSettings;
            DateTime? dealEED = swapDealSetting.EndDate;

            if (dealEED.HasValue)
                return swapDealSetting.Status == DealStatus.Inactive
                    ? swapDealSetting.DeActivationDate
                    : dealEED.Value.AddDays(swapDealSetting.GracePeriod);

            return dealEED;
        }

        #endregion
        #region Private Classes

        private class SwapDealExcelExportHandler : ExcelExportHandler<DealDefinitionDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<DealDefinitionDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Swap Deal",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Id" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Description", Width = 40 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Carrier", Width = 25 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Grace Period" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Selling Amount" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Selling Volume" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Buying Amount" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Buying Volume" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Deal Type" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Deal Contract" });

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
                                var settings = (SwapDealSettings)record.Entity.Settings;
                                row.Cells.Add(new ExportExcelCell { Value = record.Entity.DealId });
                                row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
                                row.Cells.Add(new ExportExcelCell { Value = record.CarrierAccountName });
                                row.Cells.Add(new ExportExcelCell { Value = settings.BeginDate });
                                row.Cells.Add(new ExportExcelCell { Value = settings.EndDate });
                                row.Cells.Add(new ExportExcelCell { Value = settings.GracePeriod });
                                row.Cells.Add(new ExportExcelCell { Value = settings.Inbounds.Sum(x => x.Volume * x.Rate) });
                                row.Cells.Add(new ExportExcelCell { Value = settings.Inbounds.Sum(x => x.Volume) });
                                row.Cells.Add(new ExportExcelCell { Value = settings.Outbounds.Sum(x => x.Volume * x.Rate) });
                                row.Cells.Add(new ExportExcelCell { Value = settings.Outbounds.Sum(x => x.Volume) });
                                row.Cells.Add(new ExportExcelCell { Value = Vanrise.Common.Utilities.GetEnumDescription(settings.DealType) });
                                row.Cells.Add(new ExportExcelCell { Value = Vanrise.Common.Utilities.GetEnumDescription(settings.DealContract) });
                            }
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        private class SwapDealLoggableEntity : BaseDealLoggableEntity
        {
            public static SwapDealLoggableEntity Instance = new SwapDealLoggableEntity();
            public SwapDealLoggableEntity()
            {

            }
            static SwapDealManager s_swapDealManager = new SwapDealManager();

            public override string EntityUniqueName
            {
                get { return "WhS_Deal_SwapDeal"; }
            }
            public override string EntityDisplayName
            {
                get { return "Swap Deal"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_Deal_SwapDealManager_ViewHistoryItem"; }
            }
            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                DealDefinition dealDefinition = context.Object.CastWithValidate<DealDefinition>("context.Object");
                return s_swapDealManager.GetDealName(dealDefinition);
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
            int dealId = Convert.ToInt32(context.EntityId);
            return GetDeal(dealId);
        }

        public override string GetEntityDescription(Vanrise.GenericData.Entities.IBusinessEntityDescriptionContext context)
        {
            int dealId = Convert.ToInt32(context.EntityId);
            DealDefinition deal = GetDeal(dealId);
            if (deal != null)
                return deal.Name;
            return null;
        }

        public override dynamic GetEntityId(Vanrise.GenericData.Entities.IBusinessEntityIdContext context)
        {
            var deal = context.Entity as DealDefinition;
            return deal.DealId;
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
