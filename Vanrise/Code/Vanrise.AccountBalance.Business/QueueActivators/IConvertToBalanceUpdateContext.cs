using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Business
{
    public interface IConvertToBalanceUpdateContext
    {
        dynamic Record { get; }

        Guid DataRecordTypeId { get; }

        void SubmitBalanceUpdate(BalanceUpdatePayload accountBalanceInfo);
    }
}