using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Business
{
    public class ConvertToBalanceUpdateContext : IConvertToBalanceUpdateContext
    {
        public dynamic Record { get; set; }

        public Guid DataRecordTypeId { get; set; }

        Action<BalanceUpdatePayload> _submitAccountBalanceAction;

        public ConvertToBalanceUpdateContext(Action<BalanceUpdatePayload> submitAccountBalanceAction)
        {
            _submitAccountBalanceAction = submitAccountBalanceAction;
        }

        public void SubmitBalanceUpdate(BalanceUpdatePayload accountBalanceInfo)
        {
            _submitAccountBalanceAction(accountBalanceInfo);
        }
    }
}
