using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class CompanyInfoFilter
    {
        public List<ICompanyInfoFilter> Filters { get; set; }
    }

    public interface ICompanyInfoFilter
    {
        bool IsMatch(ICompanyInfoFilterContext context); 
    }

    public interface ICompanyInfoFilterContext
    {
        int CompanyId { get; set; }
    }

    public class CompanyInfoFilterContext : ICompanyInfoFilterContext
    {
        public int CompanyId { get; set; }
    }
}
