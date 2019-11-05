using System;
using System.Linq;
using Vanrise.Common;
using Vanrise.Caching;
using Vanrise.Entities;
using TOne.WhS.Deal.Data;
using System.ComponentModel;
using TOne.WhS.Deal.Entities;
using Vanrise.Common.Business;
using System.Collections.Generic;
using TOne.WhS.Deal.Entities.Settings;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Deal.Business
{
    public class SwapDealManager : BaseDealManager
    {
        #region Public Methods
        public List<DealCapacity> GetIntersectedSwapDealsAboveCapacity(DealDefinition deal)
        {
            if (deal == null)
                return null;

            var swapDealSettings = (SwapDealSettings)deal.Settings;

            if (swapDealSettings == null || swapDealSettings.Inbounds == null || swapDealSettings.Inbounds.Count == 0 || swapDealSettings.Outbounds == null || swapDealSettings.Outbounds.Count == 0
                || swapDealSettings.BeginDate == new DateTime())
                return null;

            Dictionary<int, Interval> dealsIntervalByDealId = GetIntersectedDeals(swapDealSettings.CarrierAccountId, swapDealSettings.RealBED, swapDealSettings.RealEED);
            Dictionary<int, double> dealProgressByDealId = GetDealsReachedDurByDealId(dealsIntervalByDealId.Keys);

            var dealsCapacity = new List<DealCapacity>();
            var dealIdsByInterval = GetDealIdsByInterval(swapDealSettings, dealsIntervalByDealId);
            if (dealIdsByInterval == null || dealIdsByInterval.Count == 0)
                return dealsCapacity;

            int newDealVolume = GetDealVolume(swapDealSettings.Inbounds, swapDealSettings.Outbounds);
            double newDeallExpectedPerHour = GetExpectedPerHour(swapDealSettings.RealBED, swapDealSettings.RealEED, newDealVolume);
            int channelsLimit = new CarrierAccountManager().GetAccountChannelsLimit(swapDealSettings.CarrierAccountId);
            int nominalCapacity = channelsLimit * 60;
            var dealNames = new List<string>();

            foreach (var kvp_deal in dealIdsByInterval)
            {
                Interval dealInterval = kvp_deal.Key;
                double? dealLifeSpan = GetDealLifeSpan(dealInterval.FromDate, dealInterval.ToDate);
                double intersectedDealHours = dealInterval.ToDate.HasValue ? (dealInterval.ToDate.Value - dealInterval.FromDate).TotalHours : 0;
                double totalIntersectedExpectedCapacity = 0;
                dealNames = new List<string>();

                foreach (var dealId in kvp_deal.Value)
                {
                    DealDefinition intersectedDeal = GetDeal(dealId);
                    dealNames.Add(intersectedDeal.Name);
                    var intersectedSwapDealSettings = (SwapDealSettings)intersectedDeal.Settings;
                    var intersectedDealVolume = GetDealVolume(intersectedSwapDealSettings.Inbounds, intersectedSwapDealSettings.Outbounds);

                    double interectedReachedDuration = dealProgressByDealId.GetRecord(dealId);
                    double intersectedRemainingVolume = intersectedDealVolume - interectedReachedDuration;
                    if (dealLifeSpan.HasValue)
                    {
                        double intersectedExpectedPerHour = intersectedRemainingVolume / dealLifeSpan.Value;
                        totalIntersectedExpectedCapacity += intersectedExpectedPerHour * intersectedDealHours;
                    }
                }
                double newDealExpectedCapacity = newDeallExpectedPerHour * intersectedDealHours;
                double expectedCapacity = totalIntersectedExpectedCapacity + newDealExpectedCapacity;
                double allowedCapacity = nominalCapacity * intersectedDealHours;

                if (expectedCapacity > allowedCapacity)
                    dealsCapacity.Add(new DealCapacity
                    {
                        DealIds = kvp_deal.Value,
                        DealNames = string.Join(",", dealNames.ToArray()),
                        ExpectedCapacity = expectedCapacity,
                        FromTime = dealInterval.FromDate,
                        ToTime = dealInterval.ToDate,
                        AllowedCapacity = allowedCapacity
                    });
            }
            return dealsCapacity;
        }

        public Dictionary<int, SwapDealSettings> GetEffectiveSwapDeals(List<int> carrierAccountIds, DateTime fromDate, DateTime toDate, out DateTime? minBED, out DateTime? maxEED)
        {
            var cachedSwapDeals = new SwapDealManager().GetCachedSwapDeals();
            minBED = null;
            maxEED = null;
            if (cachedSwapDeals == null || cachedSwapDeals.Count() == 0 || carrierAccountIds == null || carrierAccountIds.Count == 0)
                return null;

            Dictionary<int, SwapDealSettings> effectiveSwapDeals = new Dictionary<int, SwapDealSettings>();

            foreach (var deal in cachedSwapDeals)
            {
                if (deal.Settings == null)
                    continue;

                var swapDealSettings = deal.Settings.CastWithValidate<SwapDealSettings>("deal.Settings");

                if (swapDealSettings.Status == DealStatus.Draft)
                    continue;
                if (!swapDealSettings.RealEED.HasValue)
                    continue;
                if (!carrierAccountIds.Contains(swapDealSettings.CarrierAccountId))
                    continue;
                if (!swapDealSettings.SendOrPay)
                    continue;

                if (swapDealSettings.RealEED.Value <= toDate && swapDealSettings.RealEED.Value >= fromDate)
                {
                    effectiveSwapDeals.Add(deal.DealId, swapDealSettings);
                    if (!minBED.HasValue || swapDealSettings.RealBED < minBED)
                        minBED = swapDealSettings.RealBED;
                    if (!maxEED.HasValue || swapDealSettings.RealEED.Value > maxEED)
                        maxEED = swapDealSettings.RealEED.Value;
                }
            }
            return effectiveSwapDeals;
        }

        public IEnumerable<DealDefinition> GetSwapDealsEffectiveAfterDate(DateTime effectiveAfter)
        {
            List<DealDefinition> dealDefinitions = new List<DealDefinition>();
            var deals = GetCachedSwapDeals();

            if (deals == null)
                return null;

            foreach (var dealDefinition in deals)
            {
                var dealSetting = dealDefinition.Settings;
                if (dealSetting.Status != DealStatus.Draft &&
                    (dealSetting.RealBED <= effectiveAfter && (!dealSetting.RealEED.HasValue || dealSetting.RealEED > effectiveAfter)))
                    dealDefinitions.Add(dealDefinition);
            }
            return dealDefinitions;
        }

        public IEnumerable<DealDefinition> GetSwapDealsBetweenDate(DateTime beginDate, DateTime? endDate)
        {
            var dealDefinitions = new List<DealDefinition>();
            var deals = GetCachedSwapDeals();

            if (deals == null)
                return null;

            foreach (var dealDefinition in deals)
            {
                if (dealDefinition.Settings.Status != DealStatus.Draft && isEffectiveDeal(beginDate, endDate, dealDefinition.Settings.RealBED, dealDefinition.Settings.RealEED))
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

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedEntities.ToBigResult(input, filterExpression, DealDefinitionDetailMapper), handler);
        }

        public DealDefinition GetSwapDealHistoryDetailbyHistoryId(int swapDealHistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var dealDefinition = s_vrObjectTrackingManager.GetObjectDetailById(swapDealHistoryId);
            return dealDefinition.CastWithValidate<DealDefinition>("DealDefinition : historyId ", swapDealHistoryId);
        }
        public DealDefinition GetSwapDealFromAnalysis(int genericBusinessEntityId)
        {
            var swapDealAnalysisManager = new SwapDealAnalysisManager();
            var swapDealAnalysisDefinitionById = swapDealAnalysisManager.GetSwapDealAnalysisSettingById();

            var swapDealAnalysisDefinition = swapDealAnalysisDefinitionById.GetRecord(genericBusinessEntityId);

            if (swapDealAnalysisDefinition == null)
                return null;

            DealDefinition dealDefinition = new DealDefinition
            {
                Name = swapDealAnalysisDefinition.Name
            };

            var analysisSetting = swapDealAnalysisDefinition.Settings;
            SwapDealSettings settings = new SwapDealSettings
            {
                CarrierAccountId = analysisSetting.CarrierAccountId,
                BeginDate = analysisSetting.FromDate,
                EEDToStore = analysisSetting.ToDate,
                Status = DealStatus.Active,
                SwapDealTimeZone = SwapDealTimeZone.Supplier
            };

            var inbounds = new List<SwapDealInbound>();
            foreach (var analysisInbound in analysisSetting.Inbounds)
            {
                var swapDealInbound = new SwapDealInbound
                {
                    ZoneGroupNumber = analysisInbound.ZoneGroupNumber,
                    CountryIds = analysisInbound.CountryIds,
                    Name = analysisInbound.GroupName,
                    Rate = analysisInbound.DealRate,
                    Volume = analysisInbound.Volume,
                    SaleZones = new List<SwapSaleZone>(),
                    SubstituteRateType = SubstituteRateType.NormalRate
                };
                foreach (var zoneId in analysisInbound.SaleZoneIds)
                {
                    swapDealInbound.SaleZones.Add(
                        new SwapSaleZone
                        {
                            ZoneId = zoneId
                        }
                    );
                }
                inbounds.Add(swapDealInbound);
            }
            settings.Inbounds = inbounds;

            var outbounds = new List<SwapDealOutbound>();
            foreach (var analysisOutbound in analysisSetting.Outbounds)
            {
                var swapDealOutbound = new SwapDealOutbound
                {
                    ZoneGroupNumber = analysisOutbound.ZoneGroupNumber,
                    CountryIds = analysisOutbound.CountryIds,
                    Name = analysisOutbound.GroupName,
                    SubstituteRateType = SubstituteRateType.NormalRate,
                    Rate = analysisOutbound.DealRate,
                    Volume = analysisOutbound.Volume,
                    SupplierZones = new List<SwapSupplierZone>()
                };
                foreach (var zoneId in analysisOutbound.SupplierZoneIds)
                {
                    swapDealOutbound.SupplierZones.Add(
                        new SwapSupplierZone
                        {
                            ZoneId = zoneId
                        }
                    );
                }
                outbounds.Add(swapDealOutbound);
            }
            settings.Outbounds = outbounds;
            dealDefinition.Settings = settings;

            return dealDefinition;
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

        public InsertDealOperationOutput<RecurredDealItem> RecurDeal(int dealId, int recurringNumber, RecurringType recurringType)
        {
            InsertDealOperationOutput<RecurredDealItem> insertOperationOutput = new InsertDealOperationOutput<RecurredDealItem>
            {
                Result = Vanrise.Entities.InsertOperationResult.Failed,
                InsertedObject = null
            };
            var deal = GetDeal(dealId);

            if (deal == null)
                throw new NullReferenceException(dealId.ToString());

            //if (!deal.Settings.IsRecurrable)
            //    throw new VRBusinessException("This Deal is not Recurrable");

            var dealDefinitionManager = new DealDefinitionManager();

            //Getting Recurred Deals
            List<DealDefinition> recurredDeals = dealDefinitionManager.GetRecurredDeals(deal, recurringNumber, recurringType);
            if (!recurredDeals.Any())
                throw new VRBusinessException("No Recurred Deals were Generated");

            //Validation
            insertOperationOutput.ValidationMessages = new List<string>();
            var errorMessages = dealDefinitionManager.ValidatingDeals(recurredDeals);
            if (errorMessages.Any())
            {
                insertOperationOutput.ValidationMessages.AddRange(errorMessages);
                return insertOperationOutput;
            }

            //Inserting
            IDealDataManager dataManager = DealDataManagerFactory.GetDataManager<IDealDataManager>();
            deal.Settings.IsRecurrable = false;

            if (dataManager.Update(deal))
            {
                RecurredDealItem recurredDealItem = new RecurredDealItem()
                {
                    UpdatedItem = DealDefinitionDetailMapper(deal)
                };
                errorMessages = new List<string>();
                recurredDealItem.InsertedItems = new List<DealDefinitionDetail>();
                foreach (var recurredDeal in recurredDeals)
                {
                    int insertedId = -1;
                    if (dataManager.Insert(recurredDeal, out insertedId))
                    {
                        CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                        insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                        recurredDealItem.InsertedItems.Add(DealDefinitionDetailMapper(recurredDeal));
                        recurredDeal.DealId = insertedId;
                    }
                    else
                    {
                        errorMessages.Add(string.Format("Deal Already exist {0}", recurredDeal.Name));
                    }
                }
                insertOperationOutput.InsertedObject = recurredDealItem;
            }
            if (errorMessages.Any())
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
                insertOperationOutput.ValidationMessages.AddRange(errorMessages);
            }

            return insertOperationOutput;
        }

        public override DealDefinitionDetail DealDefinitionDetailMapper(DealDefinition deal)
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
            detail.IsEffective = settings.RealBED <= DateTime.Now && settings.RealEED >= DateTime.Now;
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
        private Dictionary<Interval, List<int>> GetDealIdsByInterval(SwapDealSettings swapDealSettings, Dictionary<int, Interval> dealsIntervalByDealId)
        {
            HashSet<DateTime?> dates = new HashSet<DateTime?> { swapDealSettings.RealBED, swapDealSettings.RealEED };

            foreach (var dealInterval in dealsIntervalByDealId.Values)
            {
                if (dealInterval.FromDate > swapDealSettings.RealBED)
                    dates.Add(dealInterval.FromDate);

                if (dealInterval.ToDate < swapDealSettings.RealEED)
                    dates.Add(dealInterval.ToDate);
            }

            var intersectedIntervals = new List<Interval>();
            var orderedDates = dates.OrderBy(it => it).ToList();
            int count = orderedDates.Count();
            for (int i = 0; i < count - 1; i++)
            {
                intersectedIntervals.Add(new Interval
                {
                    FromDate = orderedDates[i].Value,
                    ToDate = orderedDates[i + 1]
                });
            }

            var dealIdsByInterval = new Dictionary<Interval, List<int>>();
            foreach (var dealIntersection in intersectedIntervals)
            {
                var dealIds = new List<int>();
                foreach (var dealId in dealsIntervalByDealId.Keys)
                {
                    DealDefinition intersectedDeal = GetDeal(dealId);
                    var intersectedSwapDealSettings = (SwapDealSettings)intersectedDeal.Settings;

                    if (Utilities.AreTimePeriodsOverlapped(intersectedSwapDealSettings.RealBED, intersectedSwapDealSettings.RealEED, dealIntersection.FromDate, dealIntersection.ToDate))
                        dealIds.Add(dealId);
                }
                if (dealIds.Count > 0)
                    dealIdsByInterval.Add(dealIntersection, dealIds);
            }
            return dealIdsByInterval;
        }
        private double? GetDealLifeSpan(DateTime BED, DateTime? EED)
        {
            if (!EED.HasValue)
                return null;
            return (EED.Value - BED).TotalHours;
        }
        private Dictionary<int, double> GetDealsReachedDurByDealId(IEnumerable<int> dealIds)
        {
            var dealProgressManager = new DealProgressManager();
            List<DealProgress> dealsProgress = dealProgressManager.GetDealsProgress(dealIds);
            var dealprgressDataByDealId = new Dictionary<int, double>();
            foreach (var dealProgress in dealsProgress)
            {
                if (dealProgress.CurrentTierNb == 2)
                    continue; //not including any extra volume traffic

                double reachedDuration = dealprgressDataByDealId.GetOrCreateItem(dealProgress.DealId);
                if (dealProgress.ReachedDurationInSeconds.HasValue)
                    reachedDuration += (double)dealProgress.ReachedDurationInSeconds.Value / 60;
            }
            return dealprgressDataByDealId;
        }
        public double GetExpectedPerHour(DateTime dealBED, DateTime? dealEED, int dealVolume)
        {
            double dealHours = dealEED.HasValue ? (dealEED.Value - dealBED).TotalHours : 0;
            return dealHours != 0 ? dealVolume / dealHours : 0;
        }
        private int GetDealVolume(List<SwapDealInbound> inbounds, List<SwapDealOutbound> outbounds)
        {
            int volume = 0;

            if (inbounds != null && inbounds.Count > 0)
                foreach (var inbound in inbounds)
                    volume += inbound.Volume;

            if (outbounds != null)
                foreach (var outbound in outbounds)
                    volume += outbound.Volume;

            return volume;
        }
        private Dictionary<int, Interval> GetIntersectedDeals(int carrierId, DateTime dealBED, DateTime? dealEED)
        {
            var effectiveDeals = new SwapDealManager().GetSwapDealsBetweenDate(dealBED, dealEED);

            if (effectiveDeals == null && effectiveDeals.Count() == 0)
                return null;

            var dealIntervalsByDealId = new Dictionary<int, Interval>();
            foreach (var deal in effectiveDeals)
            {
                var dealSettings = deal.Settings.CastWithValidate<SwapDealSettings>("deal.Settings");
                if (dealSettings != null)
                {
                    if (dealSettings.CarrierAccountId != carrierId)
                        continue;

                    dealIntervalsByDealId.Add(deal.DealId, new Interval { FromDate = dealSettings.RealBED, ToDate = dealSettings.RealEED });
                }
            }
            return dealIntervalsByDealId;
        }
        private bool isEffectiveDeal(DateTime fromDate, DateTime? toDate, DateTime dealBED, DateTime? DealEED)
        {
            if (!DealEED.HasValue)
                return true;

            if (toDate.HasValue)
            {
                if (dealBED < toDate && DealEED.Value > fromDate)
                    return true;
            }
            else if (DealEED.Value > fromDate)
                return true;

            return false;
        }
        #endregion

        #region Private Classes
        private class DealContext
        {
            public int DealId { get; set; }
            public DateTime DealBED { get; set; }
            public DateTime? DealEED { get; set; }
            public int DealVolume { get; set; }
            public int CarrierNominalCapacity { get; set; }
            public double ReacheDurationInMin { get; set; }
            public double NewDeallExpectedPerHour { get; set; }
        }

        private class IntersectedDeal
        {
            public DateTime BED { get; set; }
            public DateTime? EED { get; set; }
            public int DealId { get; set; }
            public int DealVolume { get; set; }
        }

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
                                row.Cells.Add(new ExportExcelCell { Value = settings.BEDToDisplay });
                                row.Cells.Add(new ExportExcelCell { Value = settings.EEDToDisplay });
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
        public struct Interval
        {
            public DateTime FromDate { get; set; }
            public DateTime? ToDate { get; set; }
        }

        #region IBusinessEntityManager

        public override List<dynamic> GetAllEntities(Vanrise.GenericData.Entities.IBusinessEntityGetAllContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetEntity(Vanrise.GenericData.Entities.IBusinessEntityGetByIdContext context)
        {
            int dealId = Convert.ToInt32(context.EntityId);
            return base.GetDeal(dealId);
        }

        public override string GetEntityDescription(Vanrise.GenericData.Entities.IBusinessEntityDescriptionContext context)
        {
            int dealId = Convert.ToInt32(context.EntityId);
            DealDefinition deal = base.GetDeal(dealId);
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