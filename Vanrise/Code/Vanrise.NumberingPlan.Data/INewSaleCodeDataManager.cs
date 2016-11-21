using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Data
{
    public interface INewSaleCodeDataManager : IDataManager, IBulkApplyDataManager<AddedCode>
    {
        long ProcessInstanceId { set; }
        void ApplyNewCodesToDB(object preparedCodes);

        
    }
}
