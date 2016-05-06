using CloudPortal.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPortal.BusinessEntity.Data
{
    public interface ICloudApplicationTypeDataManager : IDataManager
    {
        bool AreCloudApplicationTypesUpdated(ref object updateHandle);

        List<CloudApplicationType> GetAllCloudApplicationTypes();

        bool AddCloudApplicationType(CloudApplicationType cloudApplicationToAdd, out int applicationTypeId);

        bool UpdateCloudApplicationType(CloudApplicationType cloudApplicationToUpdate);
    }
}
