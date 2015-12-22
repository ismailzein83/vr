using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities.CP.Processing;

namespace TOne.WhS.CodePreparation.Business
{
    public class ChangedSaleCodeManager
    {
        public void Insert(long processInstanceID, IEnumerable<ChangedCode> changedCodes)
        {
            IChangedSaleCodeDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<IChangedSaleCodeDataManager>();
            dataManager.Insert(processInstanceID, changedCodes);
        }
    }
}
