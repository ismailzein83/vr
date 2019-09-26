using Demo.Module.Entities;
using System.Collections.Generic;
using System.Data;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class TeamDataManager : BaseSQLDataManager, ITeamDataManager
    {
        #region Properties/Ctor

        public TeamDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        #endregion

        #region Public Methods

        public List<Team> GetTeams()
        {
            return GetItemsSP<Team>("[dbo].[sp_Team_GetAll]", TeamMapper);
        }

        public bool Insert(Team team, out int insertedID)
        {
            string serializedTeamSettings = null;
            if (team.Settings != null)
                serializedTeamSettings = Serializer.Serialize(team.Settings);

            object id;
            int numberOfRowsAffected = ExecuteNonQuerySP("[dbo].[sp_Team_Insert]", out id,  team.Name, team.LeagueID, serializedTeamSettings);

            bool result = numberOfRowsAffected > 0;
            if (result)
                insertedID = (int)id;
            else
                insertedID = 0;

            return result;
        }

        public bool Update(Team team)
        {
            string serializedTeamSettings = null;
            if (team.Settings != null)
                serializedTeamSettings = Serializer.Serialize(team.Settings);

            int numberOfRowsAffected = ExecuteNonQuerySP("[dbo].[sp_Team_Update]", team.TeamID, team.Name, team.LeagueID, serializedTeamSettings);
            return numberOfRowsAffected > 0; ;
        }

        public bool AreTeamsUpdated(ref object updateHandle)
        {
            return IsDataUpdated("[dbo].[Team]", ref updateHandle);
        }

        #endregion

        #region Mapper

        private Team TeamMapper(IDataReader reader)
        {
            return new Team
            {
                TeamID = GetReaderValue<int>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name"),
                LeagueID = GetReaderValue<int>(reader, "LeagueID"),
                Settings = Serializer.Deserialize<TeamSettings>(reader["Settings"] as string)
            };
        }

        #endregion
    }
}