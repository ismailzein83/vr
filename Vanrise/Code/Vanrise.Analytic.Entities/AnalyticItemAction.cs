using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class AnalyticItemAction
    {
        public Guid ConfigId { get; set; }
        public string Title { get; set; }
        public abstract void Execute(IAnalyticItemActionContext context);
    }

    public interface IAnalyticItemActionContext
    {
    }
}