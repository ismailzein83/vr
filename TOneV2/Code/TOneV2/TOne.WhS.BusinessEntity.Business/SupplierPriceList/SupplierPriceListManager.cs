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
using Vanrise.Security.Business;
namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierPriceListManager
    {

        #region Public Methods
        public SupplierPriceList GetPriceList(int priceListId)
        {
            Dictionary<int, SupplierPriceList> priceLists = GetCachedPriceLists();
            var priceList = priceLists.GetRecord(priceListId);
            return priceList;
        }

        public Vanrise.Entities.IDataRetrievalResult<SupplierPriceListDetail> GetFilteredSupplierPriceLists(Vanrise.Entities.DataRetrievalInput<SupplierPricelistQuery> input)
        {
            Dictionary<int, SupplierPriceList> allPriceLists = GetCachedPriceLists();
            Func<SupplierPriceList, bool> filterExpression = (item) =>
                (input.Query.SupplierIds == null || input.Query.SupplierIds.Contains(item.SupplierId))
                && (input.Query.FromDate == null || item.CreateTime >= input.Query.FromDate)
                && (!input.Query.ToDate.HasValue || item.CreateTime <= input.Query.ToDate)
                && (input.Query.UserIds==null || input.Query.UserIds.Contains(item.UserId));

            ResultProcessingHandler<SupplierPriceListDetail> handler = new ResultProcessingHandler<SupplierPriceListDetail>()
            {
                ExportExcelHandler = new SupplierPriceListExcelExportHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(SupplierPriceListLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allPriceLists.ToBigResult(input, filterExpression, SupplierPriceListDetailMapper), handler);
        }

        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(GetSupplierPriceListType(), numberOfIDs, out startingId);
            return startingId;
        }


        public int GetSupplierPriceListTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetSupplierPriceListType());
        }

        public Type GetSupplierPriceListType()
        {
            return this.GetType();
        }

        #endregion

        #region Private Members

        private class SupplierPriceListExcelExportHandler : ExcelExportHandler<SupplierPriceListDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SupplierPriceListDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Supplier Pricelists",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Pricelist Type" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Supplier" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Currency" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Created Time", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.DateTime });
                
                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell() { Value = record.PricelistTypeDescription });
                            row.Cells.Add(new ExportExcelCell { Value = record.SupplierName });
                            row.Cells.Add(new ExportExcelCell { Value = record.Currency });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.CreateTime });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISupplierPriceListDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISupplierPriceListDataManager>();
            object _updateHandle;

            public override Vanrise.Caching.CacheObjectSize ApproximateObjectSize
            {
                get
                {
                    return Vanrise.Caching.CacheObjectSize.Large;
                }
            }

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.ArGetPriceListsUpdated(ref _updateHandle);
            }
        }
        public Dictionary<int, SupplierPriceList> GetCachedPriceLists()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(String.Format("GetPriceLists"),
               () =>
               {
                   ISupplierPriceListDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierPriceListDataManager>();
                   IEnumerable<SupplierPriceList> allSupplierPriceLists = dataManager.GetPriceLists();
                   Dictionary<int, SupplierPriceList> allSupplierPriceListsDic = new Dictionary<int, SupplierPriceList>();
                   if (allSupplierPriceLists != null)
                   {
                       foreach (var supplierPriceList in allSupplierPriceLists)
                       {
                           allSupplierPriceListsDic.Add(supplierPriceList.PriceListId, supplierPriceList);
                       }
                   }
                   return allSupplierPriceListsDic;
               });
        }

        private SupplierPriceListDetail SupplierPriceListDetailMapper(SupplierPriceList priceList)
        {
            SupplierPriceListDetail supplierPriceListDetail = new SupplierPriceListDetail();
            supplierPriceListDetail.Entity = priceList;
            CurrencyManager currencyManager = new CurrencyManager();
            Currency currency = currencyManager.GetCurrency(priceList.CurrencyId);
            supplierPriceListDetail.Currency = currency != null ? currency.Symbol : null;
            supplierPriceListDetail.PricelistTypeDescription = priceList.PricelistType.HasValue ? Vanrise.Common.Utilities.GetEnumDescription(priceList.PricelistType.Value) : null;
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            supplierPriceListDetail.SupplierName = carrierAccountManager.GetCarrierAccountName(priceList.SupplierId);
            UserManager userManager = new UserManager();
            supplierPriceListDetail.UserName = userManager.GetUserName(priceList.UserId);
            return supplierPriceListDetail;
        }

        #endregion
        private class SupplierPriceListLoggableEntity : VRLoggableEntityBase
        {
            public static SupplierPriceListLoggableEntity Instance = new SupplierPriceListLoggableEntity();

            private SupplierPriceListLoggableEntity()
            {

            }



            public override string EntityUniqueName
            {
                get { return "WhS_BusinessEntity_SupplierPriceList"; }
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }

            public override string EntityDisplayName
            {
                get { return "Supplier Price List"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_BusinessEntity_SupplierPriceList_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                SupplierPriceList supplierPriceList = context.Object.CastWithValidate<SupplierPriceList>("context.Object");
                return supplierPriceList.PriceListId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                return null;
            }
        }
    }
    public class PriceListFileSettings : Vanrise.Entities.VRFileExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("7042919C-A79D-4600-943F-A0BE8E3CC4F7"); }
        }

        public int PriceListId { get; set; }

        Vanrise.Security.Business.SecurityManager s_securityManager = new Vanrise.Security.Business.SecurityManager();
        public override bool DoesUserHaveViewAccess(Vanrise.Entities.IVRFileDoesUserHaveViewAccessContext context)
        {
            return s_securityManager.HasPermissionToActions("WhS_BE/SupplierPricelist/GetFilteredSupplierPricelist", context.UserId);
        }
    }
}
