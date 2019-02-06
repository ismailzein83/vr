using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.Jazz.Entities
{
    public class AccountCodeInfoFilter
    {
        public IEnumerable<IAccountCodeFilter> Filters { get; set; }

    }
    
    public interface IAccountCodeFilter
    {
        bool IsMatch(IAccountCodeFilterContext context);
    }

    public abstract class AccountCodeFilter: IAccountCodeFilter
    {
        public abstract bool IsMatch(IAccountCodeFilterContext context);

    }
    public interface IAccountCodeFilterContext
    {
        AccountCode AccountCode { get; }
    }

    public class AccountCodeFilterContext : IAccountCodeFilterContext
    {
        public AccountCode AccountCode { get; set; }
    }
}