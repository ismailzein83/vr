using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Data
{
    public interface IDealReprocessInputDataManager : IDataManager
    {
        void InsertDealReprocessInputs(List<DealReprocessInput> dealReprocessInputs); 
    }
}
