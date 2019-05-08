using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
   public class AccountManagerAnalyticPermanentFilter: AnalyticTablePermanentFilterSettings
    {
        public override Guid ConfigId => new Guid("4A3AD674-9ADB-40C6-BEFD-A1813F08F333");
        public override void ConvertToRecordFilter()
        {
            throw new NotImplementedException();
        }
    }
}
