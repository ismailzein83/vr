using System.Collections.Generic;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Data;
using Vanrise.Entities;
using System;



namespace Vanrise.Fzero.CDRImport.Data
{
    public interface ICDRDataManager : IDataManager, IBulkApplyDataManager<CDR>
    {

        void ApplyCDRsToDB(object preparedCDRs);

        void SaveCDRsToDB(List<CDR> cdrs);

        void LoadCDR(DateTime from, DateTime to, IEnumerable<string> numberPrefixes, Action<CDR> onCDRReady);

        BigResult<CDR> GetNormalCDRs(Vanrise.Entities.DataRetrievalInput<NormalCDRQuery> input);
    }
}
