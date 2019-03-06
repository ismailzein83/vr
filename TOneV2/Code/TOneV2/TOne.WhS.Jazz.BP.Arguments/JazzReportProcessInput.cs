using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
namespace TOne.WhS.Jazz.BP.Arguments
{
    public class JazzReportProcessInput :Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public override string GetTitle()
        {
            return string.Format("ERP Integration Process from {0} to {1}", FromDate.ToString(Utilities.GetDateTimeFormat(Vanrise.Entities.DateTimeType.Date)), ToDate.ToString(Utilities.GetDateTimeFormat(Vanrise.Entities.DateTimeType.Date)));
        }
    }
}
