using System.Collections.Generic;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.CDRImport.Data
{
    public interface ICDRDataManager : IDataManager
    {
        void SaveCDRsToDB(List<CDR> cdrs);
    }
}
