using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class BusinessEntityIsCacheExpiredContext : IBusinessEntityIsCacheExpiredContext
    {
       int _businessEntityDefinitionId;
       public BusinessEntityIsCacheExpiredContext(int businessEntityDefinitionId)
        {
            _businessEntityDefinitionId = businessEntityDefinitionId;
        }
 

        BusinessEntityDefinition _businessEntityDefinition;
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
