using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Data
{
    public interface ICustomerRequestTypeDataManager : IDataManager
    {
        bool AreCustomerRequestTypesUpdated(ref object updateHandle);

        List<Entities.CustomerRequestType> GetCustomerRequestTypes();
    }
}
