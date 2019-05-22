using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface IModifiedCustomerRoutePreviewDataManager : IDataManager, IBulkApplyDataManager<ModifiedCustomerRoutePreviewData>, IRoutingDataManager
    {
        void ApplyModifiedCustomerRoutesPreviewForDB(object preparedProductRoute);
        void InitializeTable();
        IEnumerable<Entities.ModifiedCustomerRoutesPreview> GetAllModifiedCustomerRoutesPreview(Vanrise.Entities.DataRetrievalInput<Entities.ModifiedCustomerRoutesPreviewQuery> input);
    }
}