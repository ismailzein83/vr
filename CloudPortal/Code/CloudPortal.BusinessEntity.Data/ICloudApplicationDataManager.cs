using CloudPortal.BusinessEntity.Entities;
using System.Collections.Generic;

namespace CloudPortal.BusinessEntity.Data
{
    public interface ICloudApplicationDataManager : IDataManager
    {
        bool AreCloudApplicationsUpdated(ref object updateHandle);

        List<CloudApplication> GetAllCloudApplications();

        bool AddCloudApplication(CloudApplicationToAdd cloudApplicationToAdd, Vanrise.Security.Entities.CloudApplicationIdentification appIdentification, out int cloudApplicationId);

        bool UpdateCloudApplication(CloudApplicationToUpdate cloudApplicationToUpdate);

        void SetApplicationReady(int cloudApplicationId);

        void DeleteCloudApplication(int cloudApplicationId);
    }
}