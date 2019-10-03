using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class LOBManager
    {
        static Guid _definitionID = new Guid("B546AFAF-EFE5-4923-927C-463DF21A491B");
        
        #region Public Methods

        public IEnumerable<LOBInfo> GetLOBInfo(LOBInfoFilter filter)
        {
            Func<LOB, bool> filterExpression = (lobEntity) =>
            {
                return true;
            };
            return GetCachedLOBs().MapRecords(LOBInfoMapper, filterExpression);
        }

        public string GetLOBName(Guid lobId)
        {
            LOB lob = GetCachedLOBs().GetRecord(lobId);
            return (lob != null) ? lob.Name : null;
        }

        #endregion

        #region Private Methods

        private Dictionary<Guid, LOB> GetCachedLOBs()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedLOB", _definitionID, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionID);
                Dictionary<Guid, LOB> result = new Dictionary<Guid, LOB>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        LOB lob = new LOB()
                        {
                            LOBId = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")
                        };
                        result.Add(lob.LOBId, lob);
                    }
                }
                return result;
            });
        }

        #endregion

        #region Mappers
        private LOBInfo LOBInfoMapper(LOB lob)
        {
            return new LOBInfo()
            {
                LOBId = lob.LOBId,
                Name =  lob.Name
            };

        }
        #endregion

        
    }
}
