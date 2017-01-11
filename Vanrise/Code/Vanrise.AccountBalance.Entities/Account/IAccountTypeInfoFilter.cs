using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.AccountBalance.Entities
{
    public  interface IAccountTypeInfoFilter
    {
        bool IsMatched(IAccountTypeInfoFilterContext context);

    }

    public interface IAccountTypeInfoFilterContext
    {
        AccountType AccountType { get; }
    }

    public class AccountTypeInfoFilterContext : IAccountTypeInfoFilterContext
    {
        public AccountType AccountType
        {
            get;
            set;
        }

       
    }
}
