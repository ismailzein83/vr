using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
   public class AccountManagerAnalyticPermanentFilter: AnalyticTablePermanentFilterSettings
    {
        public override Guid ConfigId => new Guid("4A3AD674-9ADB-40C6-BEFD-A1813F08F333");
        public override RecordFilterGroup ConvertToRecordFilter(IAnalyticTablePermanentFilterContext context)
        {
            throw new NotImplementedException();
        }
        public string TimeDimension { get; set; }
        public string CustomerDimension { get; set; }
        public string SupplierDimension { get; set; }
    }
}
