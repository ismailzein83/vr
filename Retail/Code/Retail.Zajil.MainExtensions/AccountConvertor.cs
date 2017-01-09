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
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            SettingManager settingManager = new SettingManager();
            var _systemCurrencySetting = settingManager.GetSettingByType("VR_Common_BaseCurrency");
            _currencySettingData = (CurrencySettingData)_systemCurrencySetting.Data;
            SqlSourceBatch sourceBatch = context.SourceBEBatch as SqlSourceBatch;

            List<ITargetBE> lstTargets = new List<ITargetBE>();

            foreach (DataRow row in sourceBatch.Data.Rows)
            {
                var sourceId = row["CompanyID"].ToString();
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
                lstTargets.Add(accountData);
            }

            context.TargetBEs = lstTargets;
        }

         void FillAccountSettings(SourceAccountData accountData, DataRow row)
        {
            accountData.Account.Settings = new AccountSettings
            {
                Parts = new AccountPartCollection()
            };

            FillFinancialInfo(accountData, row);
        }

         private void FillFinancialInfo(SourceAccountData accountData, DataRow row)
         {

             accountData.Account.Settings.Parts.Add(AccountPartFinancial._ConfigId, new AccountPart
             {
                 Settings = new AccountPartFinancial
                 {
                     CurrencyId = _currencySettingData.CurrencyId
                 }
             });
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
    }
}
