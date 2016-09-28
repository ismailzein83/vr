using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules
{
    public interface IRefreshRuleStateContext
    {
        DateTime EffectiveDate { get; }
    }

    public class RefreshRuleStateContext : IRefreshRuleStateContext
    {
        public DateTime EffectiveDate { get; set; }
    }
}
