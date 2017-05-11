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

                string sourceId = ((double)row[this.SourceIdColumn]).ToString();
                try
                {
                    AccountBEManager accountManager = new AccountBEManager();
                    string sourceAccountId = row[this.SourceAccountIdColumn] as string;
                    if (!string.IsNullOrEmpty(sourceAccountId))
                    {
                        Account account = accountManager.GetAccountBySourceId(AccountBEDefinitionId, sourceAccountId);
                        account.ThrowIfNull("account: sourceId", sourceAccountId);
                        Account childAccount = accountManager.GetChildAccounts(AccountBEDefinitionId, account.AccountId, false).FirstOrDefault();
                        childAccount.ThrowIfNull("childAccount: ParentId", account.AccountId);
                        accountId = childAccount.AccountId;
                    }
                    SourceDIDData didData = new SourceDIDData
                    {
                        DID = new DID
                        {
                            SourceId = sourceId,
                            Settings = new DIDSettings
                            {
                                IsInternational = (row[InternationalColumn] == DBNull.Value ? 0 : double.Parse(row[InternationalColumn].ToString())) == 1,
                                Numbers = new List<string>() { ((double)row[this.DIDColumn]).ToString() }
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
