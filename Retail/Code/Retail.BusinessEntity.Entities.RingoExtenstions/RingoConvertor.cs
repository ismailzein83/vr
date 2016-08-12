using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;

namespace Retail.BusinessEntity.Entities.RingoExtenstions
{
    public class RingoConvertor : TargetBEConvertor
    {
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            List<SourceAccountData> sourceAccounts = new List<SourceAccountData>();
            FileSourceBatch batch = context.SourceBEBatch as FileSourceBatch;

        }

        public override void MergeTargetBEs(ITargetBEConvertorMergeTargetBEsContext context)
        {

        }
    }
}
