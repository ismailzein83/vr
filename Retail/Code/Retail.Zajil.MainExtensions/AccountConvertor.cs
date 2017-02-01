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
        #region Properties

        public override string Name
        {
            get
            {
                return "Zajil Account Convertor";
            }
        }

        Currency _mainCurrency;
        public Guid AccountBEDefinitionId { get; set; }
        public Guid AccountTypeId { get; set; }
        public Guid SiteAccountTypeId { get; set; }
        public Guid InitialStatusId { get; set; }
        public Guid FinancialPartDefinitionId { get; set; }
        public Guid CompanyProfilePartDefinitionId { get; set; }
        public Guid OrderDetailsPartDefinitionId { get; set; }
        public Guid CompanyExtendedInfoPartdefinitionId { get; set; }
        #endregion

        #region Public Methods
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            _mainCurrency = currencyManager.GetSystemCurrency();
            SqlSourceBatch sourceBatch = context.SourceBEBatch as SqlSourceBatch;
            Dictionary<string, ITargetBE> zajilAccounts = new Dictionary<string, ITargetBE>();

            sourceBatch.Data.DefaultView.Sort = "CRM_Company_ID";
            foreach (DataRow row in sourceBatch.Data.Rows)
            {
                ITargetBE targetZajilAccount;
                var sourceId = row["CRM_Company_ID"].ToString();
                if (zajilAccounts.TryGetValue(sourceId, out targetZajilAccount))
                {
                    ((targetZajilAccount as SourceAccountData).Account.Settings.Parts[this.OrderDetailsPartDefinitionId].Settings as AccountPartOrderDetail).OrderDetailItems.Add(GetOrderDetailItem(row));
                    continue;
                }
                string accountName = row["Company_Name"] as string;
                SourceAccountData accountData = new SourceAccountData
                {
                    Account = new Account()
                };

                accountData.Account.Name = accountName;
                accountData.Account.SourceId = sourceId;
                accountData.Account.TypeId = this.AccountTypeId;
                FillAccountSettings(accountData, row);
                accountData.Account.StatusId = this.InitialStatusId;
                accountData.ChildrenAccounts = new List<SourceAccountData>()
                {
                    new SourceAccountData{
                     Account = new Account{
                      TypeId = this.SiteAccountTypeId,
                       Name = "Site 1",
                        StatusId = this.InitialStatusId
                     }
                    }
                };
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

            accountData.Account.Settings.Parts.Add(this.OrderDetailsPartDefinitionId, new AccountPart
            {
                Settings = new AccountPartOrderDetail
                {
                    OrderDetailItems = new List<OrderDetailItem> { GetOrderDetailItem(row) }
                }
            });
        }
        void FillFinancialInfo(SourceAccountData accountData, DataRow row)
        {

            accountData.Account.Settings.Parts.Add(this.FinancialPartDefinitionId, new AccountPart
            {
                Settings = new AccountPartFinancial
                {
                    CurrencyId = _mainCurrency.CurrencyId,
                    ProductId = 5
                }
            });
        }
        void FillCompanyProfile(SourceAccountData accountData, DataRow row)
        {

            accountData.Account.Settings.Parts.Add(this.CompanyProfilePartDefinitionId, new AccountPart
            {
                Settings = new AccountPartCompanyProfile
                {
                    Contacts = GetContactsList(row),
                    PhoneNumbers = new List<string> { row["PhoneNo"].ToString() },
                    Faxes = new List<string> { row["FaxNo"].ToString() },
                    Address = row["Address"].ToString(),
                    ArabicName = row["ArabicName"].ToString()
                }
            });
        }

        void FillCompanyExtendedInfo(SourceAccountData accountData, DataRow row)
        {

            accountData.Account.Settings.Parts.Add(this.CompanyExtendedInfoPartdefinitionId, new AccountPart
            {
                Settings = new ZajilCompanyExtendedInfo
                {
                      CRMCompanyId = row["CRM_Company_ID"].ToString(),
                      CRMCompanyAccountNo = row["CRM_Company_AccountNo"].ToString(),
                      ServiceType = row["ServiceType"].ToString(),
                      Remarks = row["Remarks"].ToString(),
                      GPVoiceCustomerNo = row["GP_VoiceCustomer_No"].ToString(),
                      ServiceId = row["ServiceID"].ToString()
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
