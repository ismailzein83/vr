using QM.CLITester.Entities;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;

namespace QM.CLITester.Data.SQL
{
    public class ProfileDataManager : BaseSQLDataManager, IProfileDataManager
    {
        public ProfileDataManager()
            : base("MainDBConnString")
        {
        }

        public Profile ProfileMapper(IDataReader reader)
        {
            Profile profile = new Profile()
            {
                ProfileId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<ProfileSettings>(reader["Settings"] as string),
                SourceId = reader["SourceProfileID"] as string
            };
            return profile;
        }

        public List<Profile> GetProfiles()
        {
            return GetItemsSP("[QM_CLITester].[sp_Profile_GetAll]", ProfileMapper);
        }

        public bool Update(Profile profile)
        {
             object settings = null;
             if ( profile.Settings != null)
               settings =  Vanrise.Common.Serializer.Serialize(profile.Settings);
             int recordsEffected = ExecuteNonQuerySP("[QM_CLITester].[sp_Profile_Update]", profile.ProfileId, profile.Name, settings);
            return (recordsEffected > 0);
        }

        public bool AreProfilesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("QM_CLITester.Profile", ref updateHandle);
        }

        public void InsertSynchronize(Profile profile)
        {
            object settings = null;
            if (profile.Settings != null)
                settings = Vanrise.Common.Serializer.Serialize(profile.Settings);

                ExecuteNonQuerySP("[QM_CLITester].[sp_Profile_InsertFromSource]", profile.ProfileId, profile.Name, profile.SourceId, settings);
        }

        public void UpdateSynchronize(Profile profile)
        {
            object settings = null;
            if (profile.Settings != null)
                settings = Vanrise.Common.Serializer.Serialize(profile.Settings);

                ExecuteNonQuerySP("[QM_CLITester].[sp_Profile_UpdateFromSource]", profile.ProfileId, profile.Name, settings);
        }
    }
}
