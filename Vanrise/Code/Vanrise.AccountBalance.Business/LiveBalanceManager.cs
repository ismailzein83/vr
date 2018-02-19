using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class LiveBalanceManager
    {
        public IEnumerable<LiveBalanceAccountInfo> GetLiveBalanceAccountsInfo(Guid accountTypeId)
        {
            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            return dataManager.GetLiveBalanceAccountsInfo(accountTypeId);
        }
        public LiveBalance GetLiveBalance(Guid accountTypeId, String accountId)
        {
            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            return dataManager.GetLiveBalance(accountTypeId, accountId);
        }
        public static string  GetBalanceFlagDescription(decimal balanceValue)
        {
            return balanceValue > 0 ? "Credit" : "Debit";
        }
        public bool CheckIfAccountHasTransactions(Guid accountTypeId, String accountId)
        {
            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            return dataManager.CheckIfAccountHasTransactions(accountTypeId, accountId);
        }
        public CurrentAccountBalance GetCurrentAccountBalance(Guid accountTypeId, String accountId)
        {
            var liveBalance = GetLiveBalance(accountTypeId,accountId);
            CurrencyManager currencyManager = new CurrencyManager();
            if (liveBalance == null)
                return null;
            int normalPression = GenericParameterManager.Current.GetNormalPrecision();

            return new CurrentAccountBalance
            {
                CurrencyDescription = currencyManager.GetCurrencySymbol(liveBalance.CurrencyId),
                CurrentBalance = Math.Round(Math.Abs(liveBalance.CurrentBalance),normalPression),
                BalanceFlagDescription = LiveBalanceManager.GetBalanceFlagDescription(liveBalance.CurrentBalance)
            };
        }

        public Vanrise.Entities.IDataRetrievalResult<AccountBalanceDetail> GetFilteredAccountBalances(Vanrise.Entities.DataRetrievalInput<AccountBalanceQuery> input)
        {
            if (input.SortByColumnName != null && input.SortByColumnName.Contains("Items"))
            {
                string[] itemProperty = input.SortByColumnName.Split('.');
                input.SortByColumnName = string.Format(@"Entity.{0}[""{1}""].Description", itemProperty[1], itemProperty[2]);
            }
            return BigDataManager.Instance.RetrieveData(input, new AccountBalanceRequestHandler());
        }

        private AccountBalanceDetail AccountBalanceDetailMapper(AccountBalanceEntity accountBalance)
        {
            return new AccountBalanceDetail {
                Entity = accountBalance,
            };
         
        }

        public bool TryUpdateLiveBalanceStatus(String accountId, Guid accountTypeId,VRAccountStatus status, bool isDeleted)
        {
            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            dataManager.TryUpdateLiveBalanceStatus(accountId, accountTypeId, status, isDeleted);
            return true;//no validation is needed
        }

        public bool TryUpdateLiveBalanceEffectiveDate(String accountId, Guid accountTypeId, DateTime? bed, DateTime? eed)
        {
            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            dataManager.TryUpdateLiveBalanceEffectiveDate(accountId, accountTypeId, bed, eed);
            return true;//no validation is needed
        }


        #region Private Classes
        private class AccountBalanceRequestHandler : BigDataRequestHandler<AccountBalanceQuery, AccountBalanceEntity, AccountBalanceDetail>
        {
            public override AccountBalanceDetail EntityDetailMapper(AccountBalanceEntity entity)
            {
                LiveBalanceManager manager = new LiveBalanceManager();
                return manager.AccountBalanceDetailMapper(entity);
            }

            public override IEnumerable<AccountBalanceEntity> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<AccountBalanceQuery> input)
            {
                ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
                IEnumerable<Vanrise.AccountBalance.Entities.AccountBalance> accountBalances = dataManager.GetFilteredAccountBalances(input.Query);
                AccountTypeSettings accountTypeSettings = new AccountTypeManager().GetAccountTypeSettings(input.Query.AccountTypeId);
                List<AccountBalanceEntity> accountBalanceDetails = new List<AccountBalanceEntity>();

                Dictionary<Guid, Object> sourceDataBySourceData = new Dictionary<Guid, Object>();

                foreach (var accountBalance in accountBalances)
                {
                    AccountBalanceEntity accountBalanceEntity = new Entities.AccountBalanceEntity
                    {
                        AccountBalanceId = accountBalance.AccountBalanceId,
                        AccountId = accountBalance.AccountId,
                        Items = new Dictionary<string, AccountBalanceDetailObject>()
                    };
                    if (accountTypeSettings != null && accountTypeSettings.AccountBalanceGridSettings != null && accountTypeSettings.AccountBalanceGridSettings.GridColumns != null)
                    {
                        foreach (var gridColumn in accountTypeSettings.AccountBalanceGridSettings.GridColumns)
                        {
                            var source = accountTypeSettings.Sources.FirstOrDefault(x => x.AccountBalanceFieldSourceId == gridColumn.SourceId);
                            if (source != null)
                            {

                                var fields = source.Settings.GetFieldDefinitions(new Business.AccountBalanceFieldSourceGetFieldDefinitionsContext { AccountTypeSettings = accountTypeSettings});
                                var field = fields.FirstOrDefault(x => x.Name == gridColumn.FieldName);
                                if (field != null)
                                {
                                    var preparedData = sourceDataBySourceData.GetOrCreateItem(gridColumn.SourceId, () =>
                                    {
                                        return source.Settings.PrepareSourceData(new AccountBalanceFieldSourcePrepareSourceDataContext { AccountTypeSettings = accountTypeSettings, AccountBalances = accountBalances, AccountTypeId = input.Query.AccountTypeId });
                                    });
                                    
                                    var fieldValue = source.Settings.GetFieldValue(new AccountBalanceFieldSourceGetFieldValueContext
                                    {
                                        FieldName = gridColumn.FieldName,
                                        AccountBalance = accountBalance,
                                        PreparedData = preparedData,
                                    });
                                    accountBalanceEntity.Items.Add(gridColumn.FieldName, new AccountBalanceDetailObject
                                    {
                                        Value = fieldValue,
                                        Description = field.FieldType.GetDescription(fieldValue)
                                    });
                                }
                            }
                        }
                        accountBalanceDetails.Add(accountBalanceEntity);
                    }
                }
                return accountBalanceDetails;
            }


            protected override ResultProcessingHandler<AccountBalanceDetail> GetResultProcessingHandler(DataRetrievalInput<AccountBalanceQuery> input, BigResult<AccountBalanceDetail> bigResult)
            {
                var resultProcessingHandler = new ResultProcessingHandler<AccountBalanceDetail>()
                {
                    ExportExcelHandler = new AccountBalanceExportExcelHandler(input.Query)
                };
                return resultProcessingHandler;
            }

        }

        private class AccountBalanceExportExcelHandler : ExcelExportHandler<AccountBalanceDetail>
        {
            AccountBalanceQuery _query;
            public AccountBalanceExportExcelHandler(AccountBalanceQuery query)
            {
                query.ThrowIfNull("Account Balance Query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<AccountBalanceDetail> context)
            {
                AccountTypeSettings accountTypeSettings = new AccountTypeManager().GetAccountTypeSettings(_query.AccountTypeId);


                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Account Balances",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                if (accountTypeSettings != null && accountTypeSettings.AccountBalanceGridSettings != null && accountTypeSettings.AccountBalanceGridSettings.GridColumns != null)
                {
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "ID", Width = 40 });

                    foreach (var gridColumn in accountTypeSettings.AccountBalanceGridSettings.GridColumns)
                    {
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = gridColumn.Title, Width = 40 });
                    }


                }
                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {

                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        if (record.Entity != null)
                        {
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.AccountBalanceId });

                            foreach (var gridColumn in accountTypeSettings.AccountBalanceGridSettings.GridColumns)
                            {
                                var item = record.Entity.Items.GetRecord(gridColumn.FieldName);
                                if (item != null)
                                    row.Cells.Add(new ExportExcelCell { Value = item.Description });
                            }
                        }
                        sheet.Rows.Add(row);
                    }
                }
                context.MainSheet = sheet;
            }
        }


        #endregion
    }
}
