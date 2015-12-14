using QM.CLITester.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
