using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class AnalyticItemAction
    {
        public abstract void Execute(IAnalyticItemActionContext context);
    }

    public interface IAnalyticItemActionContext
    {
    }

    public class OpenRecordSearchItemAction : AnalyticItemAction
    {
        public int SourceIndex { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime? ToTime { get; set; }

        public Vanrise.GenericData.Entities.RecordFilterGroup FilterGroup { get; set; }

        public override void Execute(IAnalyticItemActionContext context)
        {
            throw new NotImplementedException();
        }
    }

}