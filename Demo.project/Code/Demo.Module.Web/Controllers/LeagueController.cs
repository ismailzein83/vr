using Demo.Module.Business;
using Demo.Module.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Demo_League")]
    [JSONWithType]
    public class Demo_LeagueController : BaseAPIController
    {
        LeagueManager _leagueManager = new LeagueManager();

        [HttpPost]
        [Route("GetFilteredLeagues")]
        public object GetFilteredLeagues(DataRetrievalInput<LeagueQuery> input)
        {
            return GetWebResponse(input, _leagueManager.GetFilteredLeagues(input));
        }

        [HttpGet]
        [Route("GetLeagueByID")]
        public League GetLeagueByID(int leagueID)
        {
            return _leagueManager.GetLeagueByID(leagueID);
        }

        [HttpGet]
        [Route("GetLeaguesInfo")]
        public IEnumerable<LeagueInfo> GetLeaguesInfo(string filter = null)
        {
            LeagueInfoFilter leagueInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<LeagueInfoFilter>(filter) : null;
            return _leagueManager.GetLeaguesInfo(leagueInfoFilter);
        }

        [HttpPost]
        [Route("AddLeague")]
        public InsertOperationOutput<LeagueDetail> AddLeague(League league)
        {
            return _leagueManager.AddLeague(league);
        }

        [HttpPost]
        [Route("UpdateLeague")]
        public UpdateOperationOutput<LeagueDetail> UpdateLeague(League league)
        {
            return _leagueManager.UpdateLeague(league);
        }
    }

}