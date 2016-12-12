using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.MainExtensions.AccountParts;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.BusinessEntity.MainExtensions.FileAccountConvertors
{
    public class CompanyConvertor : TargetBEConvertor
    {
        CurrencySettingData _currencySettingData;
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            FileSourceBatch fileBatch = context.SourceBEBatch as FileSourceBatch;
            Dictionary<string, ITargetBE> targetBes = new Dictionary<string, ITargetBE>();
            SettingManager settingManager = new SettingManager();
            var _systemCurrencySetting = settingManager.GetSettingByType("VR_Common_BaseCurrency");
            _currencySettingData = (CurrencySettingData)_systemCurrencySetting.Data;

            using (Stream stream = new MemoryStream(fileBatch.Content))
            {
                TextFieldParser parser = new TextFieldParser(stream) { Delimiters = new string[] { "," } };
                while (true)
                {
                    string[] accountRecords = parser.ReadFields();
                    if (accountRecords != null)
                    {
                        var sourceId = accountRecords[4];
                        if (string.IsNullOrEmpty(sourceId))
                            continue;
                        ITargetBE targetBe;

                        if (!targetBes.TryGetValue(sourceId, out targetBe))
                        {
                            string accountName = string.Format("{0}", accountRecords[5]);

                            SourceAccountData accountData = new SourceAccountData
                            {
                                Account = new Account { TypeId = Guid.Parse("ED09FEF6-C333-400B-8F92-14FF9F8CED7B") }
                            };
                            accountData.Account.Name = accountName;
                            accountData.Account.SourceId = sourceId;

                            FillAccountSettings(accountData, accountRecords);
                            accountData.Account.StatusId = Guid.Parse("DDB6A5B8-B9E5-4050-BEE8-0F030E801B8B");
                            targetBes.Add(sourceId, accountData);
                        }

                    }
                    else
                        break;
                }
            }
            context.TargetBEs = targetBes.Values.ToList();
        }
        public override void MergeTargetBEs(ITargetBEConvertorMergeTargetBEsContext context)
        {
            SourceAccountData existingBe = context.ExistingBE as SourceAccountData;
            SourceAccountData newBe = context.NewBE as SourceAccountData;

            SourceAccountData finalBe = new SourceAccountData
            {
                Account = Serializer.Deserialize<Account>(Serializer.Serialize(existingBe.Account))
            };

            UpdatePersonalInfoPart(finalBe, newBe);


            context.FinalBE = finalBe;
        }

        void UpdatePersonalInfoPart(SourceAccountData finalBe, SourceAccountData newBe)
        {
            AccountPartPersonalInfo personalInfo = newBe.Account.Settings.Parts[AccountPartPersonalInfo._ConfigId].Settings as AccountPartPersonalInfo;
            AccountPartPersonalInfo newPersonalInfo = finalBe.Account.Settings.Parts[AccountPartPersonalInfo._ConfigId].Settings as AccountPartPersonalInfo;
            newPersonalInfo.FirstName = personalInfo.FirstName;
            newPersonalInfo.LastName = personalInfo.LastName;
            finalBe.Account.Name = string.Format("{0} {1}", newPersonalInfo.FirstName, newPersonalInfo.LastName);
            finalBe.Account.Settings.Parts[AccountPartPersonalInfo._ConfigId].Settings = newPersonalInfo;
        }

        void FillAccountSettings(SourceAccountData accountData, string[] accountRecords)
        {
            accountData.Account.Settings = new AccountSettings
            {
                Parts = new AccountPartCollection()
            };

            FillFinancialInfo(accountData, accountRecords);
        }
        void FillFinancialInfo(SourceAccountData accountData, string[] accountRecords)
        {
            accountData.Account.Settings.Parts.Add(AccountPartFinancial._ConfigId, new AccountPart
            {
                Settings = new AccountPartFinancial
                {
                    CurrencyId = _currencySettingData.CurrencyId
                }
            });
        }
    }
}
