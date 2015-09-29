using System.Collections.Generic;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Data;
using Vanrise.Entities;



namespace Vanrise.Fzero.CDRImport.Data
{
    public interface ICDRDataManager : IDataManager, IBulkApplyDataManager<CDR>
    {

        void ApplyCDRsToDB(object preparedCDRs);

        void SaveCDRsToDB(List<CDR> cdrs);
    }
}
