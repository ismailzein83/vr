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
            AccountBEManager accountManager = new AccountBEManager();
            foreach (DataRow row in sourceBatch.Data.Rows)
            {
                string sourceId = row[this.SourceIdColumn] as string;
                try
                {

                    string accountId = row[this.SourceAccountIdColumn] as string;
                    SourceDIDData didData = new SourceDIDData
                    {
                        DID = new DID
                        {
                            Number = row[this.DIDColumn] as string,
                            SourceId = sourceId,
                            Settings = new DIDSettings
                            {
                                IsInternational = (double)row["Intl_Access"] == 1
                            }
                        },
                        AccountId = accountManager.GetAccountBySourceId(AccountBEDefinitionId, accountId).AccountId
                    };
                }
                catch (Exception ex)
                {
                    var finalException = Utilities.WrapException(ex, String.Format("Failed to import DID (SourceId: '{0}') due to conversion error", sourceId));
                    context.WriteBusinessHandledException(finalException);
                }
            }
        }

        public override void MergeTargetBEs(ITargetBEConvertorMergeTargetBEsContext context)
        {
            throw new NotImplementedException();
        }
    }
}
