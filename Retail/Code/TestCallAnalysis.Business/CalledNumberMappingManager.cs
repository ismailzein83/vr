using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using TestCallAnalysis.Entities;
using Vanrise.Common;


namespace TestCallAnalysis.Business
{
    public class CalledNumberMappingManager
    {
        static Guid businessEntityDefinitionId = new Guid("0FEB4453-6951-457A-8BAA-4CB257E906C5");

        #region Public Methods
        public List<string> GetMappingNumber(long operatorID, string number)
        {
            List<string> result = new List<string>();
            var calledNumbersMapping = GetCachedCalledNumberMapping();

            foreach(CalledNumberMapping calledNumberMapping in calledNumbersMapping.Values)
            {
                if (calledNumberMapping.OperatorID == operatorID && calledNumberMapping.Number == number)
                    result.Add(calledNumberMapping.MappedNumber);
            }
            return result;
        }
        #endregion

        #region Private Methods
        private Dictionary<long, CalledNumberMapping> GetCachedCalledNumberMapping()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedCalledNumberMapping", businessEntityDefinitionId, () =>
            {
                Dictionary<long, CalledNumberMapping> result = new Dictionary<long, CalledNumberMapping>();
                IEnumerable<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(businessEntityDefinitionId);
                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        CalledNumberMapping calledNumberMapping = new CalledNumberMapping()
                        {
                            ID = (long)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            OperatorID = (long)genericBusinessEntity.FieldValues.GetRecord("OperatorID"),
                            Number = genericBusinessEntity.FieldValues.GetRecord("Number") as string,
                            MappedNumber = genericBusinessEntity.FieldValues.GetRecord("MappedNumber") as string,
                        };
                        result.Add(calledNumberMapping.ID, calledNumberMapping);
                    }
                }
                return result;
            });
        }
        #endregion

    }
}
