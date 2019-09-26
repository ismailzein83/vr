using Demo.Module.Entities;
using System.Collections.Generic;

namespace Demo.Module.Data
{
    public interface ITeamDataManager : IDataManager
    {
        List<Team> GetTeams();

        bool Insert(Team team, out int insertedId);

        bool Update(Team team);

        bool AreTeamsUpdated(ref object updateHandle);
    }
}