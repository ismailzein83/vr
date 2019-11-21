using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;

namespace TOne.WhS.RouteSync.Ericsson.Data
{
    public interface INextBTableRouteDataManager : IDataManager, IBulkApplyDataManager<NextBTableDetails>
    {
        string SwitchId { get; set; }
        
        void Initialize(INextBTableInitializeContext context);
        
        Dictionary<int, List<NextBTableDetails>> GetNextBTableDetailsByCustomerBO();
        
        HashSet<int> GetAllNextBTables();

        void InsertNextBTables(IEnumerable<NextBTableDetails> bTables);
    }
}
