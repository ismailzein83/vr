using System;
using System.Collections.Generic;
using TestCallAnalysis.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;


namespace TestCallAnalysis.Business
{
    public class CalledNumberMappingManager
    {
        static Guid businessEntityDefinitionId = new Guid("0FEB4453-6951-457A-8BAA-4CB257E906C5");

        #region Public Methods
        public IEnumerable<string> GetMappingNumber(long operatorID, string number)
        {
            var calledNumbersMapping = GetCachedCalledNumberMapping();
            Func<CalledNumberMapping, bool> filterExpression = (item) => (item.OperatorID == operatorID && item.Number == number );
            return calledNumbersMapping.MapRecords(MappingNumberMapper,filterExpression);
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
                            CalledNumberMappingId = (long)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            OperatorID = (long)genericBusinessEntity.FieldValues.GetRecord("OperatorID"),
                            Number = genericBusinessEntity.FieldValues.GetRecord("Number") as string,
                            MappedNumber = genericBusinessEntity.FieldValues.GetRecord("MappedNumber") as string,
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy"),
                        };
                        result.Add(calledNumberMapping.CalledNumberMappingId, calledNumberMapping);
                    }
                }
                return result;
            });
        }
        #endregion


        #region Mappers
        private string MappingNumberMapper(CalledNumberMapping calledNumberMapping)
        {
            return calledNumberMapping.MappedNumber;
        }
        #endregion

    }
}
