using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Data;

namespace TOne.WhS.CodePreparation.Data
{
    public interface INewSaleRateDataManager : IDataManager, IBulkApplyDataManager<AddedRate>
    {
        long ProcessInstanceId { set; }
        void ApplyNewRatesToDB(object preparedRates);

        
    }
}
