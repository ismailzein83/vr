using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace TestCallAnalysis.Business
{
    public class CaseCDREntityManager
    {
       
        public GenericBusinessEntity GetCaseCDREntity(Guid businessEntityDefinitionId, Object genericBusinessEntityId)
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            if (genericBusinessEntityId != null && businessEntityDefinitionId != null)
            {
                return genericBusinessEntityManager.GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId);
            }
            else
                return null;
        }

        public UpdateOperationOutput<GenericBusinessEntityDetail> UpdateCaseCDRStatus(GenericBusinessEntityToUpdate caseCDREntity)
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            if(caseCDREntity != null)
            {
                return genericBusinessEntityManager.UpdateGenericBusinessEntity(caseCDREntity);
            }
            else
                return null;
        }
    }
}
