using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.CP.Processing;

namespace TOne.WhS.CodePreparation.Data
{
    public interface IChangedSaleCodeDataManager:IDataManager
    {
        void Insert(long processInstanceID, IEnumerable<ChangedCode> changedCodes);
    }
}
