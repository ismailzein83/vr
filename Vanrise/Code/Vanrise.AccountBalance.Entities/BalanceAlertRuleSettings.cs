using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class BalanceAlertSettings
    {
        public List<BalanceAlertThresholdAction> ThresholdAction { get; set; }
    }

    public class BalanceAlertThresholdAction
    {
        public Decimal Threshold { get; set; }

        public BalanceAlertAction Action { get; set; }
    }

    public abstract class BalanceAlertAction
    {
        public int ConfigId { get; set; }

        public abstract void Execute(IBalanceAlertActionContext context);
    }

    public interface IBalanceAlertActionContext
    {
        Decimal Threshold { get; }

        dynamic Account { get; }
    }

    public class BalanceAlertEmailAction : BalanceAlertAction
    {
        public string To { get; set; }

        public string CC { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public override void Execute(IBalanceAlertActionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
