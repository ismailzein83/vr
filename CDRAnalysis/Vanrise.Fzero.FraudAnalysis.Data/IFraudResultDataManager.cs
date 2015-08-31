using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IFraudResultDataManager : IDataManager, IBulkApplyDataManager<SuspiciousNumber>
    {
        void ApplyFraudResultToDB(object preparedResult);
    }
}
