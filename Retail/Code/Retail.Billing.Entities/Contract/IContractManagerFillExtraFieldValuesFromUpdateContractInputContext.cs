using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public interface IContractManagerFillExtraFieldValuesFromUpdateContractInputContext
    {
        UpdateContractInput Input { get; }

        Vanrise.GenericData.Entities.GenericBusinessEntity ExistingContract { get; }

        DateTime BET { get; }

        Dictionary<string, object> EntityToUpdateFieldValues { get; }

        Dictionary<string, object> HistoryFieldValues { get; }
    }
}
