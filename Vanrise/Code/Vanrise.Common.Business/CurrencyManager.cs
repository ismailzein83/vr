using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Caching;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Entities;
namespace Vanrise.Common.Business
{
    public class CurrencyManager : BaseBusinessEntityManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<CurrencyDetail> GetFilteredCurrencies(Vanrise.Entities.DataRetrievalInput<CurrencyQuery> input)
        {
            var allCurrencies = GetCachedCurrencies();

            Func<Currency, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                  &&
                 (input.Query.Symbol == null || prod.Symbol.ToLower().Contains(input.Query.Symbol.ToLower()));

            ResultProcessingHandler<CurrencyDetail> handler = new ResultProcessingHandler<CurrencyDetail>()
            {
                ExportExcelHandler = new CurrencyExcelExportHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(CurrencyLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCurrencies.ToBigResult(input, filterExpression, CurrencyDetailMapper), handler);
        }

        public IEnumerable<Currency> GetAllCurrencies()
        {
            return this.GetCachedCurrencies().MapRecords(x => x).OrderBy(x => x.Name);
        }

        public Currency GetCurrency(int currencyId, bool isViewedFromUI)
        {
            var currencies = GetCachedCurrencies();
            var currency = currencies.GetRecord(currencyId);
            if (currency != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(CurrencyLoggableEntity.Instance, currency);
            return currency;
        }
        public Currency GetCurrency(int currencyId)
        {

            return GetCurrency(currencyId, false);
        }

        public Currency GetCurrencyBySymbol(string currencySymbol)
        {
            if (currencySymbol != null)
            {
                var currencies = GetCachedCurrenciesBySymbol();
                return currencies.GetRecord(currencySymbol.ToLower());
            }
            else
            {
                return null;
            }
          
        }

        public Currency GetCurrencyBySourceId(string sourceId)
        {
            return GetCachedCurrenciesBySourceId().GetRecord(sourceId);
        }

        public Currency GetSystemCurrency()
        {
            ConfigManager configManager = new ConfigManager();
            var systemCurrencyId = configManager.GetSystemCurrencyId();
            var systemCurrency = GetCurrency(systemCurrencyId);
            if (systemCurrency == null)
                throw new NullReferenceException(String.Format("systemCurrency ID: '{0}'", systemCurrencyId));

            return systemCurrency;
        }

        public string GetCurrencyName(int currencyId)
        {
            Currency currency = this.GetCurrency(currencyId);
            if (currency == null)
                return null;
            return currency.Name;
        }

        public string GetCurrencySymbol(int currencyId)
        {
            Currency currency = this.GetCurrency(currencyId);
            if (currency == null)
                return null;
            return currency.Symbol;
        }

        public Dictionary<int, Currency> GetCachedCurrencies()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCurrencies",
               () =>
               {
                   ICurrencyDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICurrencyDataManager>();
                   IEnumerable<Currency> currencies = dataManager.GetCurrencies();
                   return currencies.ToDictionary(cu => cu.CurrencyId, cu => cu);
               });
        }
        public Dictionary<string, Currency> GetCachedCurrenciesBySourceId()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCurrenciesBySourceId",
                () =>
                {
                    return GetCachedCurrencies().Where(v => !string.IsNullOrEmpty(v.Value.SourceId)).ToDictionary(kvp => kvp.Value.SourceId, kvp => kvp.Value);
                });
        }


        public Dictionary<string, Currency> GetCachedCurrenciesBySymbol()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCurrenciesBySymbol",
                () =>
                {
                    return GetCachedCurrencies().Where(v => !string.IsNullOrEmpty(v.Value.Symbol)).ToDictionary(kvp => kvp.Value.Symbol.ToLower(), kvp => kvp.Value);
                });
        }

        public Vanrise.Entities.InsertOperationOutput<CurrencyDetail> AddCurrency(Currency currency)
        {
            Vanrise.Entities.InsertOperationOutput<CurrencyDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CurrencyDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int currencyId = -1;

            ICurrencyDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICurrencyDataManager>();
            
            int loggedInUserId = ContextFactory.GetContext().GetLoggedInUserId();
            currency.CreatedBy = loggedInUserId;
            currency.LastModifiedBy = loggedInUserId;
            
            bool insertActionSucc = dataManager.Insert(currency, out currencyId);
            if (insertActionSucc)
            {
                currency.CurrencyId = currencyId;
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(CurrencyLoggableEntity.Instance, currency);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                var allCurrencies = GetCachedCurrencies();
                if (allCurrencies.Count == 1)
                {
                    UpdateSystemCurrency(currencyId);
                }
                insertOperationOutput.InsertedObject = CurrencyDetailMapper(currency);                    
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<CurrencyDetail> UpdateCurrency(Currency currency)
        {
            ICurrencyDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICurrencyDataManager>();

            currency.LastModifiedBy = ContextFactory.GetContext().GetLoggedInUserId();

            bool updateActionSucc = dataManager.Update(currency);
            Vanrise.Entities.UpdateOperationOutput<CurrencyDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<CurrencyDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(CurrencyLoggableEntity.Instance, currency);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = CurrencyDetailMapper(currency);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        #endregion
        public class CurrencyLoggableEntity : VRLoggableEntityBase
        {
            public static CurrencyLoggableEntity Instance = new CurrencyLoggableEntity();

            private CurrencyLoggableEntity()
            {

            }

            static CurrencyManager s_userManager = new CurrencyManager();

            public override string EntityUniqueName
            {
                get { return "VR_Common_Currency"; }
            }

            public override string ModuleName
            {
                get { return "Common"; }
            }

            public override string EntityDisplayName
            {
                get { return "Currency"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Common_Currency_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                Currency currency = context.Object.CastWithValidate<Currency>("context.Object");
                return currency.CurrencyId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                Currency currency = context.Object.CastWithValidate<Currency>("context.Object");
                return s_userManager.GetCurrencyName(currency.CurrencyId);
            }
        }
        #region Private Members

        private class CurrencyExcelExportHandler : ExcelExportHandler<CurrencyDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<CurrencyDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Currencies",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Currency Name", Width = 42 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Symbol" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Main Currency" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.CurrencyId });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Symbol });
                            row.Cells.Add(new ExportExcelCell { Value = record.IsMain });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICurrencyDataManager _dataManager = CommonDataManagerFactory.GetDataManager<ICurrencyDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCurrenciesUpdated(ref _updateHandle);
            }
        }
        private CurrencyDetail CurrencyDetailMapper(Currency currency)
        {
            CurrencyDetail currencDetail = new CurrencyDetail();

            currencDetail.Entity = currency;

            ConfigManager manager = new ConfigManager();
            if (currency.CurrencyId == manager.GetSystemCurrencyId())
                currencDetail.IsMain = true;

            return currencDetail;
        }

        private void UpdateSystemCurrency(int currencyId)
        {
            SettingManager settingManager = new SettingManager();
            var currencySettingsData = settingManager.GetSetting<CurrencySettingData>(Constants.BaseCurrencySettingType);
            SettingToEdit systemCurrency =  new SettingToEdit()
            {
                Name = "System Currency",                
                SettingId = new Guid("1c833b2d-8c97-4cdd-a1c1-c1b4d9d299de")
            };
            if (currencySettingsData == null)
            {
                systemCurrency.Data = new CurrencySettingData() { CurrencyId = currencyId };
            }
            else
            {
                currencySettingsData.CurrencyId = currencyId;
                systemCurrency.Data = currencySettingsData;
            }               
            settingManager.UpdateSetting(systemCurrency);
        }
        #endregion

        #region IBusinessEntityManager

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetCurrencySymbol(Convert.ToInt32(context.EntityId));
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var currency = context.Entity as Currency;
            return currency.CurrencyId;
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetCurrency(context.EntityId);
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            return GetAllCurrencies().Select(itm => itm as dynamic).ToList();
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
