using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleCodeManager
    {

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<SaleCodeDetail> GetFilteredSaleCodes(Vanrise.Entities.DataRetrievalInput<BaseSaleCodeQueryHandler> input)
        {
            VRActionLogger.Current.LogGetFilteredAction(SaleCodeLoggableEntity.Instance, input);
            return BigDataManager.Instance.RetrieveData(input, new SaleCodeRequestHandler());
        }

        public List<SaleCode> GetSaleCodesByZoneID(long zoneID, DateTime effectiveDate)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesByZoneID(zoneID, effectiveDate);
        }
        public List<SaleCode> GetSaleCodesByCodeGroups(List<int> codeGroupsIds)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesByCodeGroups(codeGroupsIds);
        }

        public IEnumerable<SaleCode> GetEffectiveSaleCodesByCode(int customerId, string codeNumber)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return  dataManager.GetSaleCodesByCode(codeNumber);
        }

        public List<SaleCode> GetSaleCodesByCodeId(IEnumerable<long> codeIds)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesByCodeId(codeIds);
        }
        public string GetCode(SaleCode saleCode)
        {
            if (saleCode == null)
                return null;
            return saleCode.Code;
        }

        public List<SaleCode> GetSaleCodesEffectiveByZoneID(long zoneID, DateTime effectiveDate)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesEffectiveByZoneID(zoneID, effectiveDate);
        }

        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(GetSaleCodeType(), numberOfIDs, out startingId);
            return startingId;
        }
        public List<SaleCode> GetSaleCodes(DateTime effectiveOn)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodes(effectiveOn);
        }

        public List<SaleCode> GetSaleCodesEffectiveAfter(int sellingNumberPlanId, DateTime effectiveOn, long? processInstanceId)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesEffectiveAfter(sellingNumberPlanId, effectiveOn, processInstanceId);
        }
        public List<SaleCode> GetSaleCodesByPrefix(string codePrefix, DateTime? effectiveOn, bool isFuture, bool getChildCodes, bool getParentCodes)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesByPrefix(codePrefix, effectiveOn, isFuture, getChildCodes, getParentCodes);
        }

        public IEnumerable<CodePrefixInfo> GetDistinctCodeByPrefixes(int prefixLength, DateTime? effectiveOn, bool isFuture)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetDistinctCodeByPrefixes(prefixLength, effectiveOn, isFuture);
        }
        public IEnumerable<CodePrefixInfo> GetSpecificCodeByPrefixes(int prefixLength, IEnumerable<string> codePrefixes, DateTime? effectiveOn, bool isFuture)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSpecificCodeByPrefixes(prefixLength, codePrefixes, effectiveOn, isFuture);
        }

        public List<SaleCode> GetSaleCodesByZoneName(int sellingNumberPlanId, string zoneName, DateTime effectiveDate)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesByZoneName(sellingNumberPlanId, zoneName, effectiveDate);
        }
        public List<SaleCode> GetSaleCodesEffectiveAfter(int sellingNumberPlanId, int countryId, DateTime minimumDate)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesEffectiveAfter(sellingNumberPlanId, countryId, minimumDate);
        }

        public List<SaleCode> GetSaleCodesByZoneIDs(List<long> zoneIds, DateTime effectiveDate)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesByZoneIDs(zoneIds, effectiveDate);
        }

        public int GetSaleCodeTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetSaleCodeType());
        }

        public Type GetSaleCodeType()
        {
            return this.GetType();
        }

        #endregion

        #region private Methode
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISaleCodeDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreZonesUpdated(ref _updateHandle);
            }
        }

        private SaleCodeDetail SaleCodeDetailMapper(SaleCode saleCode)
        {
            SaleCodeDetail saleCodeDetail = new SaleCodeDetail();
            SaleZoneManager szmnager = new SaleZoneManager();

            saleCodeDetail.Entity = saleCode;
            saleCodeDetail.ZoneName = szmnager.GetSaleZone(saleCode.ZoneId).Name;
            return saleCodeDetail;
        }

        #endregion

        #region Private Classes

        private class SaleCodeRequestHandler : BigDataRequestHandler<BaseSaleCodeQueryHandler, SaleCode, SaleCodeDetail>
        {
            public override SaleCodeDetail EntityDetailMapper(SaleCode entity)
            {
                SaleCodeManager manager = new SaleCodeManager();
                return manager.SaleCodeDetailMapper(entity);
            }

            public override IEnumerable<SaleCode> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<BaseSaleCodeQueryHandler> input)
            {
                return input.Query.GetFilteredSaleCodes();
            }

            protected override ResultProcessingHandler<SaleCodeDetail> GetResultProcessingHandler(DataRetrievalInput<BaseSaleCodeQueryHandler> input, BigResult<SaleCodeDetail> bigResult)
            {
                return new ResultProcessingHandler<SaleCodeDetail>
                {
                    ExportExcelHandler = new SaleCodeDetailExportExcelHandler()
                };
            }
        }

        private class SaleCodeDetailExportExcelHandler : ExcelExportHandler<SaleCodeDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SaleCodeDetail> context)
            {
                var sheet = new ExportExcelSheet()
                {
                    SheetName = "Sales Codes",
                    Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Zone", Width = 30 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Code" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.SaleCodeId });
                            row.Cells.Add(new ExportExcelCell() { Value = record.ZoneName });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.Code });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.BED });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.EED });
                            sheet.Rows.Add(row);
                        }
                    }
                }

                context.MainSheet = sheet;
            }
        }
        private class SaleCodeLoggableEntity : VRLoggableEntityBase
        {
            public static SaleCodeLoggableEntity Instance = new SaleCodeLoggableEntity();

            private SaleCodeLoggableEntity()
            {

            }

            static SaleCodeManager s_saleCodeManager = new SaleCodeManager();

            public override string EntityUniqueName
            {
                get { return "WhS_BusinessEntity_SaleCode"; }
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }

            public override string EntityDisplayName
            {
                get { return "Sale Code"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_BusinessEntity_SaleCode_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                SaleCode saleCode = context.Object.CastWithValidate<SaleCode>("context.Object");
                return saleCode.SaleCodeId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                SaleCode saleCode = context.Object.CastWithValidate<SaleCode>("context.Object");
                return s_saleCodeManager.GetCode(saleCode);
            }
        }
        #endregion

    }
}
