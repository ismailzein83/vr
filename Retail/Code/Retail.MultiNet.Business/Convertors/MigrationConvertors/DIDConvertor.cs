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


namespace Retail.MultiNet.Business.Convertors
{
    public class DIDConvertor : TargetBEConvertor
    {
        public string SourceAccountIdColumn { get; set; }
        public string DIDColumn { get; set; }
        public Guid AccountBEDefinitionId { get; set; }
        public override string Name
        {
            get
            {
                return "MultiNet DID Convertor";
            }
        }
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            SqlSourceBatch sourceBatch = context.SourceBEBatch as SqlSourceBatch;
            List<ITargetBE> transactionTargetBEs = new List<ITargetBE>();
            foreach (DataRow row in sourceBatch.Data.Rows)
            {
                long? accountId = null;

                string sourceId = null;
                try
                {
                    AccountBEManager accountManager = new AccountBEManager();
                    long sourceAccountId = row[this.SourceAccountIdColumn] != DBNull.Value ? (long)row[this.SourceAccountIdColumn] : 0;
                    if (sourceAccountId > 0)
                    {
                        Account account = accountManager.GetAccountBySourceId(AccountBEDefinitionId, string.Format("User_{0}", sourceAccountId));
                        account.ThrowIfNull("account: sourceId", sourceAccountId);
                        accountId = account.AccountId;
                    }

                    var did = row[this.DIDColumn].ToString();
                    SourceDIDData didData = new SourceDIDData
                    {
                        DID = new DID
                        {
                            SourceId = sourceId,
                            Settings = new DIDSettings
                            {
                                Numbers = new List<string>() { did.ToString() }
                            }
                        },
                        AccountId = accountId,
                        BED = new DateTime(2000, 1, 1)
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
