using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class LeagueManager
    {
        #region Public Methods

        public IDataRetrievalResult<LeagueDetail> GetFilteredLeagues(DataRetrievalInput<LeagueQuery> input)
        {
            var allLeagues = GetCachedLeagues();

            Func<League, bool> filterExpression = (league) =>
            {
                if (!string.IsNullOrEmpty(input.Query.Name) && !league.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                return true;
            };

            return DataRetrievalManager.Instance.ProcessResult(input, allLeagues.ToBigResult(input, filterExpression, LeagueDetailMapper));
        }

        public League GetLeagueByID(int leagueID)
        {
            return GetCachedLeagues().GetRecord(leagueID);
        }

        public string GetLeagueName(int leagueID)
        {
            var league = GetLeagueByID(leagueID);
            if (league == null)
                throw new NullReferenceException($"league ID: {leagueID}");

            return league.Name;
        }

        public IEnumerable<LeagueInfo> GetLeaguesInfo(LeagueInfoFilter leagueInfoFilter)
        {
            var allLeagues = GetCachedLeagues();

            Func<League, bool> filterFunc = (league) =>
            {
                if (leagueInfoFilter == null || leagueInfoFilter.Filters == null)
                    return true;

                var context = new LeagueInfoFilterContext { LeagueID = league.LeagueID };

                foreach (var filter in leagueInfoFilter.Filters)
                {
                    if (!filter.IsMatch(context))
                        return false;
                }

                return true;
            };

            return allLeagues.MapRecords(LeagueInfoMapper, filterFunc).OrderBy(league => league.Name);
        }

        public InsertOperationOutput<LeagueDetail> AddLeague(League league)
        {
            InsertOperationOutput<LeagueDetail> insertOperationOutput = new InsertOperationOutput<LeagueDetail>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            ILeagueDataManager leagueDataManager = DemoModuleFactory.GetDataManager<ILeagueDataManager>();

            int leagueID;
            bool insertActionSuccess = leagueDataManager.Insert(league, out leagueID);
            if (insertActionSuccess)
            {
                league.LeagueID = leagueID;
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = this.LeagueDetailMapper(league);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<LeagueDetail> UpdateLeague(League league)
        {
            UpdateOperationOutput<LeagueDetail> updateOperationOutput = new UpdateOperationOutput<LeagueDetail>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            ILeagueDataManager leagueDataManager = DemoModuleFactory.GetDataManager<ILeagueDataManager>();

            bool updateActionSuccess = leagueDataManager.Update(league);
            if (updateActionSuccess)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = this.LeagueDetailMapper(league);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Methods

        private Dictionary<int, League> GetCachedLeagues()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedLeagues", () =>
            {
                ILeagueDataManager leagueDataManager = DemoModuleFactory.GetDataManager<ILeagueDataManager>();
                List<League> leagues = leagueDataManager.GetLeagues();
                return leagues.ToDictionary(league => league.LeagueID, league => league);
            });
        }

        #endregion

        #region private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ILeagueDataManager leagueDataManager = DemoModuleFactory.GetDataManager<ILeagueDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return leagueDataManager.AreLeaguesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mappers

        private LeagueDetail LeagueDetailMapper(League league)
        {
            return new LeagueDetail
            {
                Name = league.Name,
                LeagueID = league.LeagueID,
                Description = league.Settings.GetDescription()
            };
        }

        private LeagueInfo LeagueInfoMapper(League league)
        {
            return new LeagueInfo
            {
                LeagueID = league.LeagueID,
                Name = league.Name
            };
        }

        #endregion
    }
}
