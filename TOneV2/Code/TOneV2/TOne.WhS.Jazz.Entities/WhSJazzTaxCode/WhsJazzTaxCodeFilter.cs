using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.Jazz.Entities
{
    public class WhSJazzTaxCodeInfoFilter
    {
        public IEnumerable<IWhSJazzTaxCodeFilter> Filters { get; set; }

    }
    public interface IWhSJazzTaxCodeFilter
    {
        bool IsMatch(IWhSJazzTaxCodeFilterContext context);
    }

    public interface IWhSJazzTaxCodeFilterContext
    {
        WhSJazzTaxCode TaxCode { get; }
    }

    public class WhSJazzTaxCodeFilterContext : IWhSJazzTaxCodeFilterContext
    {
        public WhSJazzTaxCode TaxCode { get; set; }
    }

}