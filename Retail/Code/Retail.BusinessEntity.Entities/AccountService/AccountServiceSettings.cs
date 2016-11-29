using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class AccountServiceSettings
    {
        public virtual dynamic GetFieldValue(IAccountServiceGetFieldValueContext context)
        {
            return null;
        }
    }

    public interface IAccountServiceGetFieldValueContext
    {
        string FieldName { get; }

        ServiceType ServiceType { get; }
    }
}
