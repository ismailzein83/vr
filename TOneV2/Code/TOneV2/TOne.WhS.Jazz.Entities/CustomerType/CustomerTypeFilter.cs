using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.Jazz.Entities
{
    public class CustomerTypeInfoFilter
    {
        public IEnumerable<ICustomerTypeilter> Filters { get; set; }

    }
    
    public interface ICustomerTypeilter
    {
        bool IsMatch(ICustomerTypeFilterContext context);
    }

    public interface ICustomerTypeFilterContext
    {
        CustomerType CustomerType { get; }
    }

    public class CustomerTypeFilterContext : ICustomerTypeFilterContext
    {
        public CustomerType CustomerType { get; set; }
    }
}