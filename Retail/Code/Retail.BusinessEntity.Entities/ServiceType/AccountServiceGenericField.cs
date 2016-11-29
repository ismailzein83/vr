using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class AccountServiceGenericField
    {
        public abstract string Name { get; }

        public abstract string Title { get; }

        public abstract Vanrise.GenericData.Entities.DataRecordFieldType FieldType { get; }

        public abstract dynamic GetValue(IAccountServiceGenericFieldContext context);
    }

    public interface IAccountServiceGenericFieldContext
    {
        Account Account { get; }

        AccountService AccountService { get; }
    }
}
