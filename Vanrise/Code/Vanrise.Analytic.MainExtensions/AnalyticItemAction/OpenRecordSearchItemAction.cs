using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.AnalyticItemAction
{
    public class OpenRecordSearchItemAction : Entities.AnalyticItemAction
    {
        public int SourceIndex { get; set; }
        public Vanrise.GenericData.Entities.RecordFilterGroup FilterGroup { get; set; }
        public override void Execute(IAnalyticItemActionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
