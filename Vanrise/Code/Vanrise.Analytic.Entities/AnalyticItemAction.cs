using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class AnalyticItemAction
    {
        public int ConfigId { get; set; }
        public abstract void Execute(IAnalyticItemActionContext context);
    }

    public interface IAnalyticItemActionContext
    {
    }
}