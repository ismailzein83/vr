using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities.CP.Processing;

namespace TOne.WhS.CodePreparation.Business
{
    public class NewSaleCodeManager
    {
        public void Insert(long processInstanceID, IEnumerable<AddedCode> codesList)
        {
            INewSaleCodeDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<INewSaleCodeDataManager>();
            dataManager.Insert(processInstanceID, codesList);
        }
    }
}
