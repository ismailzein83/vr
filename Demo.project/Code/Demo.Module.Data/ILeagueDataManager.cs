using Demo.Module.Entities;
using System.Collections.Generic;

namespace Demo.Module.Data
{
    public interface ILeagueDataManager : IDataManager
    {
        List<League> GetLeagues();

        bool Insert(League league, out int insertedId);

        bool Update(League league);

        bool AreLeaguesUpdated(ref object updateHandle);
    }
}