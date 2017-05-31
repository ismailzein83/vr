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
        const string fieldMessage = "Package Assignment Failed. It overlaps with other assignement";
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

        public List<AccountPackage> GetAccountPackagesByAccountId(long accountId)
        {
            var accountInfo = GetAccountInfo(accountId);
            return accountInfo != null ? accountInfo.AccountPackages : null;
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

        public void LoadAccountPackagesByPriority(Guid accountBEDefinitionId, long accountId, DateTime effectiveTime, bool withInheritence, Action<ProcessedAccountPackage, LoadPackageHandle> OnPackageLoaded)
        {
            var accountInfo = GetAccountInfo(accountId);
            if (accountInfo != null)
            {
                LoadPackageHandle handle = new LoadPackageHandle();
                foreach (var processedAccountPackage in accountInfo.AssignedPackages)
                {
                    if (processedAccountPackage.AccountPackage.IsEffective(effectiveTime))
                    {
                        OnPackageLoaded(processedAccountPackage, handle);
                        if (handle.Stop)
                            return;
                    }
                    else if (processedAccountPackage.AccountPackage.BED < effectiveTime)
                    {
                        break;
                    }
                }
                if (withInheritence && accountInfo.Account.ParentAccountId.HasValue)
                    LoadAccountPackagesByPriority(accountBEDefinitionId, accountInfo.Account.ParentAccountId.Value, effectiveTime, withInheritence, OnPackageLoaded);
            }
            else
            {
                if (withInheritence)
                {
                    var account = _accountBEManager.GetAccount(accountBEDefinitionId, accountId);
                    account.ThrowIfNull("account", accountBEDefinitionId);

                    if (account.ParentAccountId.HasValue)
                        LoadAccountPackagesByPriority(accountBEDefinitionId, account.ParentAccountId.Value, effectiveTime, withInheritence, OnPackageLoaded);
                }
            }
        }

        public InsertOperationOutput<AccountPackageDetail> AddAccountPackage(AccountPackageToAdd accountPackageToAdd)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AccountPackageDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
         
            var package = _packageManager.GetPackage(accountPackageToAdd.PackageId);
            var account = _accountBEManager.GetAccount(accountPackageToAdd.AccountBEDefinitionId,accountPackageToAdd.AccountId);

            PackageSettingAssignementValidateContext packageSettingAssignementValidateContext = new Entities.PackageSettingAssignementValidateContext
            {
                Account = account,
                AccountId =accountPackageToAdd.AccountId ,
                BED=accountPackageToAdd.BED,
                EED=accountPackageToAdd.EED,

            };
            package.Settings.ExtendedSettings.ValidatePackageAssignment(packageSettingAssignementValidateContext);

            if (packageSettingAssignementValidateContext.IsValid)
            {
                if (IsOverLappedAccoutPackage(accountPackageToAdd.AccountPackageId, accountPackageToAdd.AccountId, accountPackageToAdd.PackageId, accountPackageToAdd.BED, accountPackageToAdd.EED))
                {
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
                    insertOperationOutput.Message = fieldMessage;
                    insertOperationOutput.ShowExactMessage = true;
                }
                else
                {
                    IAccountPackageDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageDataManager>();
                    int accountPackageId = -1;
                    if (dataManager.Insert(accountPackageToAdd, out accountPackageId))
                    {
                        var packageName = _packageManager.GetPackageName(accountPackageToAdd.PackageId);
                        Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                        accountPackageToAdd.AccountPackageId = accountPackageId;
                        VRActionLogger.Current.LogObjectCustomAction(new Retail.BusinessEntity.Business.AccountBEManager.AccountBELoggableEntity(accountPackageToAdd.AccountBEDefinitionId), "Assign Package", true, account, String.Format("Account -> Package {0} {1} {2}", packageName, accountPackageToAdd.BED, accountPackageToAdd.EED));
                        insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                        insertOperationOutput.InsertedObject = this.AccountPackageDetailMapper(accountPackageToAdd.AccountBEDefinitionId, accountPackageToAdd);
                    }
                    else
                    {
                        insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
                    }
                }
            }else
            {
                insertOperationOutput.Message = packageSettingAssignementValidateContext.ErrorMessage;
            }
            return insertOperationOutput;
        }

        public UpdateOperationOutput<AccountPackageDetail> UpdateAccountPackage(AccountPackageToEdit accountPackageToEdit)
        {
            var accountPackage = GetAccountPackage(accountPackageToEdit.AccountPackageId);
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountPackageDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            var accountBEDefinitionId = _packageManager.GetPackageAccountDefinitionId(accountPackage.PackageId);
            var package = _packageManager.GetPackage(accountPackage.PackageId);
            var account = _accountBEManager.GetAccount(accountBEDefinitionId, accountPackage.AccountId);

            PackageSettingAssignementValidateContext packageSettingAssignementValidateContext = new Entities.PackageSettingAssignementValidateContext
            {
                Account = account,
                AccountId = accountPackage.AccountId,
                BED = accountPackageToEdit.BED,
                EED = accountPackageToEdit.EED,

            };
            package.Settings.ExtendedSettings.ValidatePackageAssignment(packageSettingAssignementValidateContext);
            if (packageSettingAssignementValidateContext.IsValid)
            {
                if (IsOverLappedAccoutPackage(accountPackageToEdit.AccountPackageId, accountPackage.AccountId, accountPackage.PackageId, accountPackageToEdit.BED, accountPackageToEdit.EED))
                {
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
                    updateOperationOutput.Message = fieldMessage;
                    updateOperationOutput.ShowExactMessage = true;
                }
                else
                {
                    IAccountPackageDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageDataManager>();
                    if (dataManager.Update(accountPackageToEdit))
                    {
                        var packageName = _packageManager.GetPackageName(accountPackage.PackageId);
                        Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                        VRActionLogger.Current.LogObjectCustomAction(new Retail.BusinessEntity.Business.AccountBEManager.AccountBELoggableEntity(accountBEDefinitionId), "Update AccountPackage", true, account, String.Format("Account -> Package {0} {1} {2}", packageName, accountPackageToEdit.BED, accountPackageToEdit.EED));
                        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                        updateOperationOutput.UpdatedObject = AccountPackageDetailMapper(_packageManager.GetPackageAccountDefinitionId(accountPackage.PackageId), this.GetAccountPackage(accountPackageToEdit.AccountPackageId));
                    }
                    else
                    {
                        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
                    }
                }
            }else
            {
                updateOperationOutput.Message = packageSettingAssignementValidateContext.ErrorMessage;
            }
            
            return updateOperationOutput;
        }

        public bool IsOverLappedAccoutPackage(int accountPackageId, long accountId, int packageId, DateTime bed, DateTime? eed)
        {
            var accountPackages = GetAccountPackagesByAccountId(accountId);
            if (accountPackages != null)
            {
                foreach (var accountPackage in accountPackages)
                {
                    if (accountPackage.PackageId == packageId && accountPackage.AccountPackageId != accountPackageId)
                    {
                        if (Utilities.AreTimePeriodsOverlapped(bed, eed, accountPackage.BED, accountPackage.EED))
                            return true;
                    }
                }
            }
            return false;
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
        public bool DoesUserHaveEditAccountPackageAccess(int accountPackageId)
        {
            var accountpackage = GetAccountPackage(accountPackageId);
            var accountBEDefinitionId = _packageManager.GetPackageAccountDefinitionId(accountpackage.PackageId);
            return DoesUserHaveAddAccountPackageAccess(accountBEDefinitionId);
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

            //DateTime? _accountCacheLastCheck;
            //AccountBEManager.CacheManager _accountCacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<AccountBEManager.CacheManager>();
            DateTime? _packageCacheLastCheck;
            PackageManager.CacheManager _packageCacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<PackageManager.CacheManager>();

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAccountPackagesUpdated(ref _updateHandle) 
                    //|
                    //_accountCacheManager.IsCacheExpired(ref _accountCacheLastCheck)
                    |
                   _packageCacheManager.IsCacheExpired(ref _packageCacheLastCheck); 
            }
        }

        private class AccountInfo
        {
            public Account Account { get; set; }

            List<AccountPackage> _accountPackages = new List<AccountPackage>();
            public List<AccountPackage> AccountPackages
            {
                get
                {
                    return _accountPackages;
                }
            }

            public IOrderedEnumerable<ProcessedAccountPackage> AssignedPackages { get; set; }
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
                  AccountBEManager accountBEManager = new AccountBEManager();

                  Dictionary<long, AccountInfo> accountInfos = new Dictionary<long, AccountInfo>();
                  Dictionary<int, Package> allPackages = new PackageManager().GetCachedPackages();
                  Dictionary<int, Guid> accountDefinitionIdsByPackageId = GetAccountDefinitionIdsByPackageId(allPackages);
                  Dictionary<long, List<ProcessedAccountPackage>> accountPackages = new Dictionary<long, List<ProcessedAccountPackage>>();
                  foreach (var accountPackage in GetCachedAccountPackages().Values)
                  {
                      Package package = allPackages.GetRecord(accountPackage.PackageId);
                      if (package != null)
                      {
                          AccountInfo accountInfo;
                          if (!accountInfos.TryGetValue(accountPackage.AccountId, out accountInfo))
                          {
                              Account account = accountBEManager.GetAccount(accountDefinitionIdsByPackageId[accountPackage.PackageId], accountPackage.AccountId);
                              if (account == null)
                                  continue;
                              accountInfo = new AccountInfo { Account = account };
                              accountInfos.Add(accountPackage.AccountId, accountInfo);
                          }
                          accountInfo.AccountPackages.Add(accountPackage);
                          accountPackages.GetOrCreateItem(accountPackage.AccountId).Add(new ProcessedAccountPackage { AccountPackage = accountPackage, Package = package });
                      }
                  }
                  foreach (var accountInfo in accountInfos.Values)
                  {
                      accountInfo.AssignedPackages = accountPackages[accountInfo.Account.AccountId].OrderByDescending(itm => itm.AccountPackage.EED.HasValue ? itm.AccountPackage.EED.Value : DateTime.MaxValue);
                  }
                  return accountInfos;
              });
        }

        private Dictionary<int, Guid> GetAccountDefinitionIdsByPackageId(Dictionary<int, Package> allPackages)
        {
            Dictionary<int, Guid> accountDefinitionIdsByPackageId = new Dictionary<int, Guid>();
            PackageDefinitionManager packageDefinitionManager = new PackageDefinitionManager();
            foreach (var package in allPackages.Values)
            {
                package.Settings.ThrowIfNull("package.Settings", package.PackageId);
                var packageDefinition = packageDefinitionManager.GetPackageDefinitionById(package.Settings.PackageDefinitionId);
                packageDefinition.ThrowIfNull("packageDefinition", package.Settings.PackageDefinitionId);
                packageDefinition.Settings.ThrowIfNull("packageDefinition.Settings", package.Settings.PackageDefinitionId);
                accountDefinitionIdsByPackageId.Add(package.PackageId, packageDefinition.Settings.AccountBEDefinitionId);
            }
            return accountDefinitionIdsByPackageId;
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

    public class LoadPackageHandle
    {
        public bool Stop { get; set; }
    }

    public class ProcessedAccountPackage
    {
        public AccountPackage AccountPackage { get; set; }

        public Package Package { get; set; }
    }
}
