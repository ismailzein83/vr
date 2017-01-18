using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.MainExtensions.AccountParts;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Retail.Zajil.MainExtensions
{
    public class AccountConvertor : TargetBEConvertor
    {
        CurrencySettingData _currencySettingData;
        #region Public Methods
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            SettingManager settingManager = new SettingManager();
            var _systemCurrencySetting = settingManager.GetSettingByType("VR_Common_BaseCurrency");
            _currencySettingData = (CurrencySettingData)_systemCurrencySetting.Data;
            SqlSourceBatch sourceBatch = context.SourceBEBatch as SqlSourceBatch;
            Dictionary<string, ITargetBE> zajilAccounts = new Dictionary<string, ITargetBE>();

            sourceBatch.Data.DefaultView.Sort = "CRM_Company_ID";
            foreach (DataRow row in sourceBatch.Data.Rows)
            {
                ITargetBE targetZajilAccount;
                var sourceId = row["CRM_Company_ID"].ToString();
                if (zajilAccounts.TryGetValue(sourceId, out targetZajilAccount))
                {
                    ((targetZajilAccount as SourceAccountData).Account.Settings.Parts[AccountPartOrderDetail._ConfigId].Settings as AccountPartOrderDetail).OrderDetailItems.Add(GetOrderDetailItem(row));
                    continue;
                }
                string accountName = row["Company_Name"] as string;
                SourceAccountData accountData = new SourceAccountData
                {
                    Account = new Account()
                };

                accountData.Account.Name = accountName;
                accountData.Account.SourceId = sourceId;
                accountData.Account.TypeId = new Guid("046078A0-3434-4934-8F4D-272608CFFEBF");
                FillAccountSettings(accountData, row);
                accountData.Account.StatusId = Guid.Parse("DDB6A5B8-B9E5-4050-BEE8-0F030E801B8B");
                zajilAccounts.Add(sourceId, accountData);
            }
            context.TargetBEs = zajilAccounts.Values.ToList();
        }
        public override void MergeTargetBEs(ITargetBEConvertorMergeTargetBEsContext context)
        {
            SourceAccountData existingBe = context.ExistingBE as SourceAccountData;
            SourceAccountData newBe = context.NewBE as SourceAccountData;

            SourceAccountData finalBe = new SourceAccountData
            {
                Account = Serializer.Deserialize<Account>(Serializer.Serialize(existingBe.Account))
            };

            finalBe.Account.TypeId = newBe.Account.TypeId;
            context.FinalBE = finalBe;
        }
        #endregion

        #region Private Methods
        void FillAccountSettings(SourceAccountData accountData, DataRow row)
        {
            accountData.Account.Settings = new AccountSettings
            {
                Parts = new AccountPartCollection()
            };
            FillCompanyProfile(accountData, row);
            FillOrderDetails(accountData, row);
            FillFinancialInfo(accountData, row);
        }
        void FillOrderDetails(SourceAccountData accountData, DataRow row)
        {

            accountData.Account.Settings.Parts.Add(AccountPartOrderDetail._ConfigId, new AccountPart
            {
                Settings = new AccountPartOrderDetail
                {
                    OrderDetailItems = new List<OrderDetailItem> { GetOrderDetailItem(row) }
                }
            });
        }
        OrderDetailItem GetOrderDetailItem(DataRow row)
        {
            return new OrderDetailItem
            {
                Achievement = row["Achievement"].ToString(),
                Charges = row["Charges"].ToString(),
                ChargesYear1 = GetDecimalValue(row, "Charges_Year1"),
                ChargesYear2 = GetDecimalValue(row, "Charges_Year2"),
                ChargesYear3 = GetDecimalValue(row, "Charges_Year3"),
                ContractDays = GetDecimalValue(row, "contract_days"),
                ContractPeriod = GetDecimalValue(row, "ContractPeriod"),
                ContractRemain = GetDecimalValue(row, "ContractRemain"),
                Discount = row["Discount"].ToString(),
                Installation = GetDecimalValue(row, "Installation"),
                Payment = row["Payment"].ToString(),
                ThirdParty = GetDecimalValue(row, "ThirdParty"),
                TotalContract = GetDecimalValue(row, "total_contract")
            };
        }
        decimal GetDecimalValue(DataRow row, string columnName)
        {
            decimal result = 0;
            decimal.TryParse(row[columnName].ToString(), out result);
            return result;
        }
        void FillFinancialInfo(SourceAccountData accountData, DataRow row)
        {

            accountData.Account.Settings.Parts.Add(AccountPartFinancial._ConfigId, new AccountPart
            {
                Settings = new AccountPartFinancial
                {
                    CurrencyId = _currencySettingData.CurrencyId
                }
            });
        }
        void FillCompanyProfile(SourceAccountData accountData, DataRow row)
        {

            accountData.Account.Settings.Parts.Add(AccountPartCompanyProfile._ConfigId, new AccountPart
            {
                Settings = new AccountPartCompanyProfile
                {
                    Contacts = GetContactsList(row),
                    PhoneNumbers = new List<string> { row["PhoneNo"].ToString() },
                    Faxes = new List<string> { row["FaxNo"].ToString() },
                    Street = row["Address"].ToString()
                }
            });
        }
        List<AccountCompanyContact> GetContactsList(DataRow row)
        {
            List<AccountCompanyContact> contacts = new List<AccountCompanyContact>();

            contacts.Add(new AccountCompanyContact
            {
                ContactType = "Finance",
                ContactName = row["finance_contact_person"].ToString(),
                Email = row["finance_contact_email"].ToString(),
                PhoneNumbers = new List<string> { row["finance_contact_number"].ToString() }
            });

            return contacts;
        }
        #endregion
    }
}
