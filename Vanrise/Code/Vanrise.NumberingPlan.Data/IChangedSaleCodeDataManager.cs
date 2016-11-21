using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Data
{
    public interface IChangedSaleCodeDataManager : IDataManager, IBulkApplyDataManager<ChangedCode>
    {
        long ProcessInstanceId { set; }
     
        void ApplyChangedCodesToDB(object preparedObject);
    }
}
