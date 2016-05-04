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

        bool Insert(CloudApplicationToAdd cloudApplicationToAdd, Vanrise.Security.Entities.CloudApplicationIdentification appIdentification, out int applicationId);

        void SetApplicationReady(int applicationId);
    }
}
