using Demo.Module.Business;
using Demo.Module.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Demo_Team")]
    [JSONWithType]
    public class Demo_TeamController : BaseAPIController
    {
        TeamManager _teamManager = new TeamManager();

        [HttpPost]
        [Route("GetFilteredTeams")]
        public object GetFilteredTeams(DataRetrievalInput<TeamQuery> input)
        {
            return GetWebResponse(input, _teamManager.GetFilteredTeams(input));
        }

        [HttpGet]
        [Route("GetTeamByID")]
        public Team GetTeamByID(int teamID)
        {
            return _teamManager.GetTeamByID(teamID);
        }

        [HttpGet]
        [Route("GetTeamSettingsConfigs")]
        public IEnumerable<TeamSettingsConfig> GetTeamSettingsConfigs()
        {
            return _teamManager.GetTeamSettingsConfigs();
        }

        [HttpGet]
        [Route("GetPlayerTypeConfigs")]
        public IEnumerable<PlayerTypeConfig> GetPlayerTypeConfigs()
        {
            return _teamManager.GetPlayerTypeConfigs();
        }

        [HttpPost]
        [Route("AddTeam")]
        public InsertOperationOutput<TeamDetail> AddTeam(Team team)
        {
            return _teamManager.AddTeam(team);
        }

        [HttpPost]
        [Route("UpdateTeam")]
        public UpdateOperationOutput<TeamDetail> UpdateTeam(Team team)
        {
            return _teamManager.UpdateTeam(team);
        }
    }
}