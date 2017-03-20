using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;

namespace Retail.BusinessEntity.Business
{
    public class AccountPackageManager
    {
        #region Fields

        AccountBEManager _accountBEManager = new AccountBEManager();
        PackageManager _packageManager = new PackageManager();

        #endregion



        #region Public Methods

        public IDataRetrievalResult<AccountPackageDetail> GetFilteredAccountPackages(DataRetrievalInput<AccountPackageQuery> input)
        {
            Dictionary<int, AccountPackage> cachedAccountPackages = this.GetCachedAccountPackages();
            Func<AccountPackage, bool> filterExpression = (accountPackage) => (accountPackage.AccountId == input.Query.AssignedToAccountId);

            ResultProcessingHandler<AccountPackageDetail> handler = new ResultProcessingHandler<AccountPackageDetail>()
            {
                ExportExcelHandler = new AccountPackageExcelExportHandler()
            };

            return DataRetrievalManager.Instance.ProcessResult(input, cachedAccountPackages.ToBigResult(input, filterExpression,
                    (accountPackage) => AccountPackageDetailMapper(input.Query.AccountBEDefinitionId, accountPackage)), handler);
        }

        public AccountPackage GetAccountPackage(int accountPackageId)
        {
            Dictionary<int, AccountPackage> cachedAccountPackages = this.GetCachedAccountPackages();
            return cachedAccountPackages.GetRecord(accountPackageId);
        }
        public int GetAccountPackagesCount(long accountId)
        {
            var accountInfo = GetAccountInfo(accountId);
            if (accountInfo != null)
                return accountInfo.AccountPackages.Count;
            else
                return 0;
        }

        public IEnumerable<int> GetPackageIdsAssignedToAccount(long accountId)
        {
            var accountInfo = GetAccountInfo(accountId);
            if (accountInfo != null)
                return accountInfo.AccountPackages.MapRecords(itm => itm.PackageId);
            else
                return new List<int>();
        }

        public IEnumerable<int> GetPackageIdsAssignedToAccount(long accountId, DateTime effectiveTime)
        {
            var accountInfo = GetAccountInfo(accountId);

            Func<AccountPackage, bool> filterPredicate = (itm) =>
            {
                if (itm.BED > effectiveTime || (itm.EED.HasValue && itm.EED <= effectiveTime))
                    return false;

                return true;
            };

            if (accountInfo != null && accountInfo.AccountPackages != null)
                return accountInfo.AccountPackages.MapRecords(itm => itm.PackageId, filterPredicate);

            return null;
        }

        public InsertOperationOutput<AccountPackageDetail> AddAccountPackage(AccountPackageToAdd accountPackageToAdd)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AccountPackageDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IAccountPackageDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageDataManager>();
            int accountPackageId = -1;

            if (dataManager.Insert(accountPackageToAdd, out accountPackageId))
            {var account = _accountBEManager.GetAccount(accountPackageToAdd.AccountBEDefinitionId, accountPackageToAdd.AccountId);
                var accountName=_accountBEManager.GetAccountName(accountPackageToAdd.AccountBEDefinitionId, accountPackageToAdd.AccountId);
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                accountPackageToAdd.AccountPackageId = accountPackageId;
                VRActionLogger.Current.LogObjectCustomAction(new Retail.BusinessEntity.Business.AccountBEManager.AccountBELoggableEntity(accountPackageToAdd.AccountBEDefinitionId), "Assign Package", true, account, String.Format("Account -> Package {0} {1} {2}", accountName, accountPackageToAdd.BED, accountPackageToAdd.EED));
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = this.AccountPackageDetailMapper(accountPackageToAdd.AccountBEDefinitionId, accountPackageToAdd);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public bool DoesUserHaveViewAccountPackageAccess(Guid accountBEDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return new AccountBEDefinitionManager().DoesUserHaveViewAccountPackageAccess(userId, accountBEDefinitionId);
        }

        public bool DoesUserHaveAddAccountPackageAccess(Guid accountBEDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return new AccountBEDefinitionManager().DoesUserHaveAddAccountPackageAccess(userId, accountBEDefinitionId);
        }

        #endregion

        #region Private Classes


        private class AccountPackageExcelExportHandler : ExcelExportHandler<AccountPackageDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<AccountPackageDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Packages",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Package", Width = 20 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.AccountPackageId });
                            row.Cells.Add(new ExportExcelCell { Value = record.PackageName });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.BED });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.EED });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IAccountPackageDataManager _dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAccountPackagesUpdated(ref _updateHandle);
            }
        }

        private class AccountInfo
        {
            List<AccountPackage> _accountPackages = new List<AccountPackage>();
            public List<AccountPackage> AccountPackages
            {
                get
                {
                    return _accountPackages;
                }
            }
        }

        #endregion

        #region Private Methods

        Dictionary<int, AccountPackage> GetCachedAccountPackages()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAccountPackages", () =>
            {
                IAccountPackageDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageDataManager>();
                IEnumerable<AccountPackage> accountPackages = dataManager.GetAccountPackages();
                return accountPackages.ToDictionary(kvp => kvp.AccountPackageId, kvp => kvp);
            });
        }

        private AccountInfo GetAccountInfo(long accountId)
        {
            return GetCachedAccountInfoByAccountId().GetRecord(accountId);
        }

        private Dictionary<long, AccountInfo> GetCachedAccountInfoByAccountId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAccountInfoByAccountId",
              () =>
              {
                  Dictionary<long, AccountInfo> accountInfos = new Dictionary<long, AccountInfo>();
                  foreach (var accountPackage in GetCachedAccountPackages().Values)
                  {
                      accountInfos.GetOrCreateItem(accountPackage.AccountId).AccountPackages.Add(accountPackage);
                  }
                  return accountInfos;
              });
        }



        #endregion

        #region Mappers

        AccountPackageDetail AccountPackageDetailMapper(Guid accountBEDefinitionId, AccountPackage accountPackage)
        {

            return new AccountPackageDetail()
            {
                Entity = accountPackage,
                AccountName = _accountBEManager.GetAccountName(accountBEDefinitionId, accountPackage.AccountId),
                PackageName = _packageManager.GetPackageName(accountPackage.PackageId)
            };
        }

        #endregion
    }
}
