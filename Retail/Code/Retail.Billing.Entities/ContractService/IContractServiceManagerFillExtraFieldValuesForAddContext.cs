using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public interface IContractServiceManagerFillExtraFieldValuesForAddContext
    {
        AddContractServiceInput Input { get; }

        DateTime BET { get; }

        Dictionary<string, object> EntityToAddFieldValues { get; }

        Dictionary<string, object> HistoryFieldValues { get; }
    }
}
