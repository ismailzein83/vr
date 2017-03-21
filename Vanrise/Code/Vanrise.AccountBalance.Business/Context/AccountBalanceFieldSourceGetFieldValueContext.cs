using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class AccountBalanceFieldSourceGetFieldValueContext : IAccountBalanceFieldSourceGetFieldValueContext
    {
        public object PreparedData { get; set; }

        public Entities.AccountBalance AccountBalance { get; set; }

        public string FieldName { get; set; }


        
    }
}
