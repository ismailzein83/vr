﻿using System;
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
                var sourceId = row["CRM_Company_ID"] as string;
                string accountName = row["Company_Name"] as string;
                try
                {
                    if (zajilAccounts.TryGetValue(sourceId, out targetZajilAccount))
                    {
                        ((targetZajilAccount as SourceAccountData).Account.Settings.Parts[this.OrderDetailsPartDefinitionId].Settings as AccountPartOrderDetail).OrderDetailItems.Add(GetOrderDetailItem(row));
                        continue;
                    }
                    SourceAccountData accountData = new SourceAccountData
                    {
                        Account = new Account()
                    };

                    accountData.Account.Name = accountName;
                    accountData.Account.CreatedTime = row["CreateDate"] != DBNull.Value ? (DateTime)row["CreateDate"] : default(DateTime);
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
                catch (Exception ex)
                {
                    var finalException = Utilities.WrapException(ex, String.Format("Failed to import Account (Id: '{0}' Name: '{1}') due to conversion error", sourceId, accountName));
                    context.WriteBusinessHandledException(finalException);
                }
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
            UpdateOrderPart(finalBe, newBe);
            context.FinalBE = finalBe;
        }

        void UpdateOrderPart(SourceAccountData finalBe, SourceAccountData newBe)
        {
            AccountPartOrderDetail newAccountPartOrderDetail = newBe.Account.Settings.Parts[this.OrderDetailsPartDefinitionId].Settings as AccountPartOrderDetail;
            finalBe.Account.Settings.Parts[this.OrderDetailsPartDefinitionId].Settings = newAccountPartOrderDetail;
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
            FillCompanyExtendedInfo(accountData, row);
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
                    PhoneNumbers = new List<string> { row["PhoneNo"] as string },
                    Faxes = new List<string> { row["FaxNo"] as string },
                    Address = row["Address"] as string,
                    ArabicName = row["Arabic_Name"] as string
                }
            });
        }

        void FillCompanyExtendedInfo(SourceAccountData accountData, DataRow row)
        {

            accountData.Account.Settings.Parts.Add(this.CompanyExtendedInfoPartdefinitionId, new AccountPart
            {
                Settings = new ZajilCompanyExtendedInfo
                {
                    CRMCompanyId = row["CRM_Company_ID"] as string,
                    CRMCompanyAccountNo = row["CRM_Company_AccountNo"] as string,
                    SalesAgent = row["SalesAgent"] as string,
                    ServiceType = row["ServiceType"] as string,
                    CompanyId = GetIntValue(row, "CompanyID"),
                    GPVoiceCustomerNo = row["GP_VoiceCustomer_No"] as string,
                    ServiceId = row["ServiceID"] as string,
                    CustomerPO = row["customer_po"] as string
                }
            });
        }
        OrderDetailItem GetOrderDetailItem(DataRow row)
        {
            return new OrderDetailItem
            {
                OrderId = (int)row["OrderId"],
                Achievement = row["Achievement"] as string,
                Charges = row["Charges"] as string,
                ChargesYear1 = GetDoubleValue(row, "Charges_Year1"),
                ChargesYear2 = GetDoubleValue(row, "Charges_Year2"),
                ChargesYear3 = GetDoubleValue(row, "Charges_Year3"),
                ContractDays = GetDoubleValue(row, "contract_days"),
                ContractPeriod = GetDoubleValue(row, "ContractPeriod"),
                ContractRemain = row["ContractRemain"] as string,
                Discount = row["Discount"] as string,
                Installation = GetDoubleValue(row, "Installation"),
                Payment = row["Payment"] as string,
                ThirdParty = GetDoubleValue(row, "ThirdParty"),
                TotalContract = GetDoubleValue(row, "total_contract"),
                ParentSo = GetDoubleValue(row, "ParentSO"),
                ChildSo = row["ChildSO"] as string,
                SalesAgent = row["SalesAgent"] as string,
                Type = row["Type"] as string,
                ConfirmDate = GetDateValue(row, "ConfirmDate"),
                CloseDate = GetDateValue(row, "CloseDate"),
                Remarks = row["Remarks"] as string,
                SONo = (int)row["SO_NO"]
            };
        }
        decimal GetDecimalValue(DataRow row, string columnName)
        {
            return row[columnName] == DBNull.Value ? 0 : decimal.Parse(row[columnName].ToString());
        }

        double GetDoubleValue(DataRow row, string columnName)
        {
            return row[columnName] == DBNull.Value ? 0 : double.Parse(row[columnName].ToString());
        }
        int GetIntValue(DataRow row, string columnName)
        {
            return row[columnName] == DBNull.Value ? 0 : int.Parse(row[columnName].ToString());
        }
        DateTime? GetDateValue(DataRow row, string columnName)
        {
            return row[columnName] == DBNull.Value ? null  : (DateTime?)DateTime.Parse(row[columnName].ToString());
        }

        Dictionary<string, AccountCompanyContact> GetContactsList(DataRow row)
        {
            Dictionary<string, AccountCompanyContact> contacts = new Dictionary<string, AccountCompanyContact>();

            contacts.Add("Main", new AccountCompanyContact
            {
                ContactName = row["Contact_Person"] as string,
                Email = row["Email"] as string,
                PhoneNumbers = new List<string> { row["Mobile"] as string },
                Title = row["title"] as string
            });

            contacts.Add("Finance", new AccountCompanyContact
            {
                ContactName = row["finance_contact_person"] as string,
                Email = row["finance_contact_email"] as string,
                PhoneNumbers = new List<string> { row["finance_contact_number"] as string },
            });

            return contacts;
        }

        #endregion
    }
}
