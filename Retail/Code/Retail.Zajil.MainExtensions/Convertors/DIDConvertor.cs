using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;

namespace Retail.Zajil.MainExtensions.Convertors
{
    public class DIDConvertor : TargetBEConvertor
    {
        public string SourceIdColumn { get; set; }
        public string SourceAccountIdColumn { get; set; }
        public string BEDColumn { get; set; }
        public string DIDColumn { get; set; }
        public string InternationalColumn { get; set; }
        public string DIDSoColumn { get; set; }
        public string NumberOfChannelsColumn { get; set; }
        public Guid AccountBEDefinitionId { get; set; }

        public override string Name
        {
            get
            {
                return "Zajil DID Convertor";
            }
        }
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            SqlSourceBatch sourceBatch = context.SourceBEBatch as SqlSourceBatch;
            List<ITargetBE> transactionTargetBEs = new List<ITargetBE>();
            foreach (DataRow row in sourceBatch.Data.Rows)
            {
                long? accountId = null;

                string sourceId = ((int)row[this.SourceIdColumn]).ToString();
                try
                {
                    AccountBEManager accountManager = new AccountBEManager();
                    string sourceAccountId = row[this.SourceAccountIdColumn] as string;
                    var didSO = row[DIDSoColumn] == DBNull.Value ? null : (int?)row[DIDSoColumn].ToString().TryParseWithValidate<int>(int.TryParse);

                    if (!string.IsNullOrEmpty(sourceAccountId))
                    {
                        var siteAccountId = string.Format("Site_{0}_{1}", sourceAccountId, didSO);
                        Account account = accountManager.GetAccountBySourceId(AccountBEDefinitionId, sourceAccountId);
                        account.ThrowIfNull("account: sourceId", sourceAccountId);
                        Account childAccount = accountManager.GetAccountBySourceId(AccountBEDefinitionId, siteAccountId);
                        childAccount.ThrowIfNull("childAccount: ParentId", account.AccountId);
                        accountId = childAccount.AccountId;
                    }

                    var isInternational = row[InternationalColumn] == DBNull.Value ? false : row[InternationalColumn].ToString().TryParseWithValidate<bool>(bool.TryParse);
                    var did = row[this.DIDColumn].ToString().TryParseWithValidate<double>(double.TryParse);
                    SourceDIDData didData = new SourceDIDData
                    {
                        DID = new DID
                        {
                            SourceId = sourceId,
                            Settings = new DIDSettings
                            {
                                IsInternational = isInternational,
                                Numbers = new List<string>() { did.ToString() },
                                DIDSo = didSO,
                                NumberOfChannels = row[NumberOfChannelsColumn] == DBNull.Value ? 0 : (int)row[NumberOfChannelsColumn]
                            }
                        },
                        AccountId = accountId,
                        BED = row[BEDColumn] == DBNull.Value ? default(DateTime?) : (DateTime?)row[BEDColumn]
                    };
                    transactionTargetBEs.Add(didData);
                }
                catch (Exception ex)
                {
                    var finalException = Utilities.WrapException(ex, String.Format("Failed to import DID (SourceId: '{0}') due to conversion error", sourceId));
                    context.WriteBusinessHandledException(finalException);
                }
            }
            context.TargetBEs = transactionTargetBEs;
        }

        public override void MergeTargetBEs(ITargetBEConvertorMergeTargetBEsContext context)
        {
            SourceDIDData existingBe = context.ExistingBE as SourceDIDData;
            SourceDIDData newBe = context.NewBE as SourceDIDData;

            SourceDIDData finalBe = new SourceDIDData
            {
                DID = newBe.DID,
                BED = newBe.BED,
                AccountId = newBe.AccountId
            };
            finalBe.DID.DIDId = existingBe.DID.DIDId;
            context.FinalBE = finalBe;
        }
    }
}
