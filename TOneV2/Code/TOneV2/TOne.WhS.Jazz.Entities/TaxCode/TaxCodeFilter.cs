using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.Jazz.Entities
{
    public class TaxCodeInfoFilter
    {
        public IEnumerable<ITaxCodeFilter> Filters { get; set; }

    }
    public interface ITaxCodeFilter
    {
        bool IsMatch(ITaxCodeFilterContext context);
    }

    public interface ITaxCodeFilterContext
    {
        TaxCode TaxCode { get; }
    }

    public class TaxCodeFilterContext : ITaxCodeFilterContext
    {
        public TaxCode TaxCode { get; set; }
    }

}