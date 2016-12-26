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
        public abstract Guid ConfigId {get;}

        public virtual dynamic GetFieldValue(IAccountPartGetFieldValueContext context)
        {
            return null;
        }
    }

    public interface IAccountPartGetFieldValueContext
    {
        string FieldName { get; }

        AccountPartDefinition PartDefinition { get; }
    }

    public class AccountPartCollection : Dictionary<Guid, AccountPart>
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
