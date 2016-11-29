using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountServiceGetFieldValueContext : IAccountServiceGetFieldValueContext
    {
        string _fieldName;
        ServiceType _serviceType;
        public AccountServiceGetFieldValueContext(string fieldName, ServiceType serviceType)
        {
            _fieldName = fieldName;
            _serviceType = serviceType;
        }

        public string FieldName
        {
            get { return _fieldName; }
        }

        public ServiceType ServiceType
        {
            get { return _serviceType; }
        }
    }
}