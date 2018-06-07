using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class FamilyInfoFilter
    {
        public List<IFamilyInfoFilter> Filters { get; set; }
    }
    public interface IFamilyInfoFilter
    {
        bool IsMatch(IFamilyInfoFilterContext context);
    }

    public interface IFamilyInfoFilterContext
    {
        long FamilyId { get; set; }
    }
    public class FamilyInfoFilterContext : IFamilyInfoFilterContext
    {
        public long FamilyId { get; set; }
    }
}
