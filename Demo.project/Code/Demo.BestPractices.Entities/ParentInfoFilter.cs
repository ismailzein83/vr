using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BestPractices.Entities
{
    public class ParentInfoFilter
    {
        public List<IParentInfoFilter> Filters { get; set; }
    }
    public interface IParentInfoFilter
    {
        bool IsMatch(IParentInfoFilterContext context);
    }

    public interface IParentInfoFilterContext
    {
        long ParentId { get; set; }
    }
    public class ParentInfoFilterContext : IParentInfoFilterContext
    {
        public long ParentId { get; set; }
    }
}
