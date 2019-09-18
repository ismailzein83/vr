using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class LeagueInfoFilter
    {
        public List<ILeagueInfoFilter> Filters { get; set; }
    }

    public interface ILeagueInfoFilter
    {
        bool IsMatch(ILeagueInfoFilterContext context);
    }

    public interface ILeagueInfoFilterContext
    {
        int LeagueId { get; set; }
    }

    public class LeagueInfoFilterContext : ILeagueInfoFilterContext
    {
        public int LeagueId { get; set; }
    }
}
