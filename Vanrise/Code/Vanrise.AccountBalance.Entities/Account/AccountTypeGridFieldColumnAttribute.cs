using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountTypeGridFieldColumnAttribute : GridColumnAttribute
    {
        public int? FieldColor { get; set; }
    }
}
