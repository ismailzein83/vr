using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountServiceTypeGenericField : AccountServiceGenericField
    {
        ServiceType _serviceType;
        GenericFieldDefinition _fieldDefinition;
        public AccountServiceTypeGenericField(ServiceType serviceType, GenericFieldDefinition fieldDefinition)
        {
            _serviceType = serviceType;
            _fieldDefinition = fieldDefinition;
        }

        public override string Name
        {
            get { return _fieldDefinition.Name; }
        }

        public override string Title
        {
            get { return _fieldDefinition.Title; }
        }

        public override Vanrise.GenericData.Entities.DataRecordFieldType FieldType
        {
            get { return _fieldDefinition.FieldType; }
        }

        public override dynamic GetValue(IAccountServiceGenericFieldContext context)
        {
            if (context.AccountService.Settings != null)
                return context.AccountService.Settings.GetFieldValue(new AccountServiceGetFieldValueContext(_fieldDefinition.Name, _serviceType));
            else
                return null;
        }
    }
}
