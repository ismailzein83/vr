using CloudPortal.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPortal.BusinessEntity.Data
{
    public interface ICloudApplicationDataManager : IDataManager
    {
        bool AreApplicationsUpdated(ref object updateHandle);

        List<CloudApplication> GetAllApplications();
    }
}
