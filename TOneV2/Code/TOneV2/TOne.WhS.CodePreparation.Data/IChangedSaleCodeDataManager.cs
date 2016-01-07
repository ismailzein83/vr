using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.CP.Processing;
using Vanrise.Data;

namespace TOne.WhS.CodePreparation.Data
{
    public interface IChangedSaleCodeDataManager : IDataManager, IBulkApplyDataManager<ChangedCode>
    {
        long ProcessInstanceId { set; }
        void Insert(long processInstanceID, IEnumerable<ChangedCode> changedCodes);

        void ApplyChangedCodesToDB(object preparedObject, long processInstanceID);
    }
}
