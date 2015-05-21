using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface ISuspiciousNumberDataManager : IDataManager 
    {
        void SaveSuspiciousNumbers(List<SuspiciousNumber> suspiciousNumbers);
        void SaveNumberProfiles(List<NumberProfile> numberProfiles);
    }
}
