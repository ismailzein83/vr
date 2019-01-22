using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.Jazz.Entities
{
    public class WhSJazzCustomerTypeCodeInfoFilter
    {
        public IEnumerable<IWhSJazzCustomerTypeCodeFilter> Filters { get; set; }

    }
    
    public interface IWhSJazzCustomerTypeCodeFilter
    {
        bool IsMatch(IWhSJazzCustomerTypeCodeFilterContext context);
    }

    public interface IWhSJazzCustomerTypeCodeFilterContext
    {
        WhSJazzCustomerTypeCode CustomerTypeCode { get; }
    }

    public class WhSJazzCustomerTypeCodeFilterContext : IWhSJazzCustomerTypeCodeFilterContext
    {
        public WhSJazzCustomerTypeCode CustomerTypeCode { get; set; }
    }
}