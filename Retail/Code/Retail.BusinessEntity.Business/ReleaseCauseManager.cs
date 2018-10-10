using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class ReleaseCauseManager
    {
        static Guid beDefinitionId = new Guid("A37FE810-1E62-4699-B16C-DA97CFEAEF49");

        public bool IsReleaseCodeDelivered(string releaseCode)
        {
            if (string.IsNullOrEmpty(releaseCode))
                return false;

            Dictionary<string, ReleaseCause> releaseCausesByCode = GetCachedReleaseCausesByCode();
            ReleaseCause releaseCause = releaseCausesByCode.GetRecord(releaseCode);
            return releaseCause != null && releaseCause.IsDelivered;
        }

        private Dictionary<string, ReleaseCause> GetCachedReleaseCausesByCode()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedReleaseCauses", beDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(beDefinitionId);
                Dictionary<string, ReleaseCause> result = new Dictionary<string, ReleaseCause>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        ReleaseCause releaseCause = new ReleaseCause()
                        {
                            ReleaseCauseId = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            ReleaseCode = genericBusinessEntity.FieldValues.GetRecord("ReleaseCode") as string,
                            ReleaseCodeName = genericBusinessEntity.FieldValues.GetRecord("ReleaseCodeName") as string,
                            IsDelivered = (bool)genericBusinessEntity.FieldValues.GetRecord("IsDelivered"),
                            Description = (string)genericBusinessEntity.FieldValues.GetRecord("Description"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int?)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime?)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int?)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")
                        };
                        result.Add(releaseCause.ReleaseCode, releaseCause);
                    }
                }

                return result;
            });
        }
    }
}
