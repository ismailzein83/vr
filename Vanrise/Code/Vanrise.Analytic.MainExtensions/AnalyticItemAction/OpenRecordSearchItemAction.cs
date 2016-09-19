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
        Guid _configId;
        public override Guid ConfigId { get{ return _configId;} set { _configId = new Guid("E2A332A2-74FA-4C42-A5D1-33FBDA093946");}}
        public int ReportId { get; set; }
        public string SourceName { get; set; }
        public override void Execute(IAnalyticItemActionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
