using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;

namespace  Demo.Module.Data
{
    public interface IServiceTypeDataManager : IDataManager
    {
        IEnumerable<ServiceType> GetAllServiceTypes();

        
        bool AreServiceTypeUpdated(ref object updateHandle);
    }
}
