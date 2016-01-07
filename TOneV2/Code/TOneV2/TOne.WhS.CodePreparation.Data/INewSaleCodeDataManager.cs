using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.CP.Processing;
using Vanrise.Data;

namespace TOne.WhS.CodePreparation.Data
{
    public interface INewSaleCodeDataManager : IDataManager, IBulkApplyDataManager<AddedCode>
    {
        long ProcessInstanceId { set; }
        void Insert(long processInstanceID, IEnumerable<AddedCode> codesList);

        void ApplyNewCodesToDB(object preparedCodes, long processInstanceID);

        
    }
}
