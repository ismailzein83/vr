using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class TeamManager
    {
        LeagueManager _leagueManager = new LeagueManager();

        #region Public Methods

        public IDataRetrievalResult<TeamDetail> GetFilteredTeams(DataRetrievalInput<TeamQuery> input)
        {
            var allTeams = GetCachedTeams();

            Func<Team, bool> filterExpression = (team) =>
            {
                if (!string.IsNullOrEmpty(input.Query.Name) && !team.Name.ToLower().Contains(input.Query.Name))
                    return false;

                if (input.Query.LeagueIDs != null && !input.Query.LeagueIDs.Contains(team.LeagueID))
                    return false;

                return true;
            };

            return DataRetrievalManager.Instance.ProcessResult(input, allTeams.ToBigResult(input, filterExpression, TeamDetailMapper));
        }

        public Team GetTeamByID(int teamID)
        {
            return GetCachedTeams().GetRecord(teamID);
        }

        public IEnumerable<TeamSettingsConfig> GetTeamSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<TeamSettingsConfig>(TeamSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<PlayerTypeConfig> GetPlayerTypeConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<PlayerTypeConfig>(PlayerTypeConfig.EXTENSION_TYPE);
        }

        public InsertOperationOutput<TeamDetail> AddTeam(Team team)
        {
            InsertOperationOutput<TeamDetail> insertOperationOutput = new InsertOperationOutput<TeamDetail>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            ITeamDataManager teamDataManager = DemoModuleFactory.GetDataManager<ITeamDataManager>();

            int teamID;
            bool insertActionSuccess = teamDataManager.Insert(team, out teamID);
            if (insertActionSuccess)
            {
                team.TeamID = teamID;
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = TeamDetailMapper(team);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;

        }

        public UpdateOperationOutput<TeamDetail> UpdateTeam(Team team)
        {
            UpdateOperationOutput<TeamDetail> updateOperationOutput = new UpdateOperationOutput<TeamDetail>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            ITeamDataManager teamDataManager = DemoModuleFactory.GetDataManager<ITeamDataManager>();

            bool insertActionSuccess = teamDataManager.Update(team);
            if (insertActionSuccess)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = TeamDetailMapper(team);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;

        }

        #endregion

        #region Private Methods

        private Dictionary<int, Team> GetCachedTeams()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedTeams", () =>
            {
                ITeamDataManager teamDataManager = DemoModuleFactory.GetDataManager<ITeamDataManager>();
                List<Team> teams = teamDataManager.GetTeams();
                return teams.ToDictionary(team => team.TeamID, team => team);
            });
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ITeamDataManager teamDataManager = DemoModuleFactory.GetDataManager<ITeamDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return teamDataManager.AreTeamsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mappers

        private TeamDetail TeamDetailMapper(Team team)
        {
            return new TeamDetail
            {
                TeamID = team.TeamID,
                Name = team.Name,
                LeagueName = _leagueManager.GetLeagueName(team.LeagueID),
                Description = team.Settings != null ? team.Settings.GetDescription() : null
            };
        }

        #endregion
    }
}