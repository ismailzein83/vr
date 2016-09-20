using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class AnalyticItemAction
    {
        public abstract Guid ConfigId { get; }
        public string Title { get; set; }
        public abstract void Execute(IAnalyticItemActionContext context);
    }

    public interface IAnalyticItemActionContext
    {
    }
}