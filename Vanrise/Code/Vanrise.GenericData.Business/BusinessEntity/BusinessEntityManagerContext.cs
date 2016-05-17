using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class BusinessEntityGetAllContext : BusinessEntityManagerContext, IBusinessEntityGetAllContext
    {
        public BusinessEntityGetAllContext(int businessEntityDefinitionId)
            : base(businessEntityDefinitionId)
        {
        }
        public BusinessEntityGetAllContext(BusinessEntityDefinition businessEntityDefinition)
            : base(businessEntityDefinition)
        {
        }
    }

    public class BusinessEntityIsCacheExpiredContext : BusinessEntityManagerContext, IBusinessEntityIsCacheExpiredContext
    {
        public BusinessEntityIsCacheExpiredContext(int businessEntityDefinitionId)
            : base(businessEntityDefinitionId)
        {
        }
        public BusinessEntityIsCacheExpiredContext(BusinessEntityDefinition businessEntityDefinition)
            : base(businessEntityDefinition)
        {
        }
    }

    public class BusinessEntityGetByIdContext : BusinessEntityManagerContext, IBusinessEntityGetByIdContext
    {
        public BusinessEntityGetByIdContext(int businessEntityDefinitionId)
            : base(businessEntityDefinitionId)
        {
        }
        public BusinessEntityGetByIdContext(BusinessEntityDefinition businessEntityDefinition)
            : base(businessEntityDefinition)
        {
        }

        public dynamic EntityId
        {
            get;
            set;
        }
    }

    public abstract class BusinessEntityManagerContext
    {
        int _businessEntityDefinitionId;
        BusinessEntityDefinition _businessEntityDefinition;

        public BusinessEntityManagerContext(int businessEntityDefinitionId)
        {
            _businessEntityDefinitionId = businessEntityDefinitionId;
        }

        public BusinessEntityManagerContext(BusinessEntityDefinition businessEntityDefinition)
        {
            if (businessEntityDefinition == null)
                throw new ArgumentNullException("businessEntityDefinition");
            _businessEntityDefinition = businessEntityDefinition;
            _businessEntityDefinitionId = businessEntityDefinition.BusinessEntityDefinitionId;
        }

        public BusinessEntityDefinition EntityDefinition
        {
            get
            {
                if(_businessEntityDefinition == null)
                {
                    _businessEntityDefinition = (new BusinessEntityDefinitionManager()).GetBusinessEntityDefinition(_businessEntityDefinitionId);
                    if (_businessEntityDefinition == null)
                        throw new NullReferenceException(String.Format("_businessEntityDefinition '{0}'", _businessEntityDefinitionId));
                }
                return _businessEntityDefinition;
            }
        }

        public int EntityDefinitionId
        {
            get { return _businessEntityDefinitionId; }
        }
    }
}
