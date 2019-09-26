using Demo.Module.Entities;
using System.Collections.Generic;
using System.Data;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class LeagueDataManager: BaseSQLDataManager, ILeagueDataManager
    {
        #region Properties/Ctor

        public LeagueDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        #endregion


        #region Public Methods

        public List<League> GetLeagues()
        {
            return GetItemsSP("[dbo].[sp_League_GetAll]", LeagueMapper);
        }

        public bool Insert(League league, out int insertedId)
        {
            string serializedLeagueSettings = null;
            if (league.Settings != null)
                serializedLeagueSettings = Serializer.Serialize(league.Settings);

            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_League_Insert]", out id, league.Name, serializedLeagueSettings);
            
            bool result = nbOfRecordsAffected > 0;
            if (result)
                insertedId = (int)id;
            else
                insertedId = 0;

            return result;
        }

        public bool Update(League league)
        {
            string serializedLeagueSettings = null;
            if (league.Settings != null)
                serializedLeagueSettings = Serializer.Serialize(league.Settings);

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_League_Update]", league.LeagueID, league.Name, serializedLeagueSettings);
            return nbOfRecordsAffected > 0;
        }

        public bool AreLeaguesUpdated(ref object updateHandle)
        {
            return IsDataUpdated("[dbo].[League]", ref updateHandle);
        }

        #endregion

        #region Mappers

        private League LeagueMapper(IDataReader reader)
        {
            return new League
            {
                LeagueID = GetReaderValue<int>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name"),
                Settings = Serializer.Deserialize<LeagueSettings>(reader["Settings"] as string)
            };
        }

        #endregion

    }
}