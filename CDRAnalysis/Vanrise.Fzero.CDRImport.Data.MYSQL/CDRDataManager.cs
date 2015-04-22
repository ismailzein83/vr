using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Data;

namespace Vanrise.Fzero.CDRImport.Data.MYSQL
{
    public class CDRDataManager : BaseDataManager, ICDRDataManager
    {
        public CDRDataManager()
            : base("CDRDBConnectionString")
        {

        }


        public object PrepareCDRsForDBApply(List<CDR> cdrs)
        {
            throw new NotImplementedException();
        }

        public void ApplyCDRsToDB(object preparedInvalidCDRs)
        {
            throw new NotImplementedException();
        }
    }
}
