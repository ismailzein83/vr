using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public interface IAdapterImportDataContext
    {
        int DataSourceId { get; }

        BaseAdapterArgument AdapterArgument { get; }
        
        void GetStateWithLock(Func<BaseAdapterState, BaseAdapterState> onStateReady);

        void OnDataReceived(IImportedData data);

        void StartNewInstanceIfAllowed();
    }
}
