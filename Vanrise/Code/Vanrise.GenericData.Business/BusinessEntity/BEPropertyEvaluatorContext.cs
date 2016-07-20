using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class BEPropertyEvaluatorContext : IBEPropertyEvaluatorContext
    {
        public BEPropertyEvaluatorContext(int businessEntityDefinitionId, dynamic businessEntityObject)
        {
            if (businessEntityObject == null)
                throw new ArgumentNullException("businessEntityObject");
            _businessEntityDefinitionId = businessEntityDefinitionId;
            _businessEntityObject = businessEntityObject;
        }

        int _businessEntityDefinitionId;
        public int BusinessEntityDefinitionId
        {
            get { return _businessEntityDefinitionId; }
        }

        dynamic _businessEntityObject;
        public dynamic BusinessEntityObject
        {
            get { return _businessEntityObject; }
        }
    }
}
