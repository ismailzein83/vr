using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.CDRImport.Data
{
    public interface ICDRDataManager : IDataManager
    {
        Object PrepareCDRsForDBApply(List<CDR> cdrs);

        void ApplyCDRsToDB(Object preparedInvalidCDRs);
    }
}
