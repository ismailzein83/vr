using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCallAnalysis.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace TestCallAnalysis.Business
{
    public class WhiteListManager
    {
        static Guid businessEntityDefinitionId = new Guid("9BB6ADE7-0DA9-4C49-BA0B-7837007B1EC2");

        #region Public Methods
        public List<string> GetWhiteListByOperatorId(long operatorID)
        {
            List<string> result = new List<string>();
            var whiteList = GetCachedWhiteList();

            foreach (WhiteList whtList in whiteList.Values)
            {
                if (whtList.OperatorID == operatorID)
                    result.Add(whtList.Number);
            }
            return result;
        }
        #endregion

        #region Private Methods
        private Dictionary<long, WhiteList> GetCachedWhiteList()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedWhiteList", businessEntityDefinitionId, () =>
            {
                Dictionary<long, WhiteList> result = new Dictionary<long, WhiteList>();
                IEnumerable<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(businessEntityDefinitionId);
                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        WhiteList whiteList = new WhiteList()
                        {
                            WhiteListId = (long)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            OperatorID = (long)genericBusinessEntity.FieldValues.GetRecord("OperatorId"),
                            Number = genericBusinessEntity.FieldValues.GetRecord("Number") as string,
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy"),
                        };
                        result.Add(whiteList.WhiteListId, whiteList);
                    }
                }
                return result;
            });
        }
        #endregion

    }
}
