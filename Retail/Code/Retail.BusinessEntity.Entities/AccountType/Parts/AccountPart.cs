using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountPart
    {
        //public string PartUniqueName { get; set; }

        public AccountPartSettings Settings { get; set; }
    }

    public abstract class AccountPartSettings
    {
       
    }

    public class AccountPartCollection : Dictionary<int, AccountPart>
    {
        //public AccountPart TryGet(string partUniqueName)
        //{
        //    AccountPart accountPart;
        //    if (this.TryGetValue(partUniqueName, out accountPart))
        //        return accountPart;
        //    else
        //        return null;
        //}
    }
}
