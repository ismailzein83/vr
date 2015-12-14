using QM.CLITester.Entities;
using System.Collections.Generic;

namespace QM.CLITester.Data
{
    public interface IProfileDataManager : IDataManager
    {
        List<Profile> GetProfiles();

        bool Update(Profile profile);

        void InsertSynchronize(Profile profile);

        void UpdateSynchronize(Profile profile);

        bool AreProfilesUpdated(ref object updateHandle);
    }
}
