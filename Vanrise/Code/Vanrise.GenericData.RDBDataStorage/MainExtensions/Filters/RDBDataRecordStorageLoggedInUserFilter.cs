using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.GenericData.RDBDataStorage.MainExtensions.Filters
{
    public class RDBDataRecordStorageLoggedInUserFilter : RDBDataRecordStorageSettingsFilter
    {
        GenericBusinessEntityDefinitionManager _genericBEDefinitionManager;
        public override Guid ConfigId { get { return new Guid("CF3123E6-1A84-4A8A-873B-DF7DBF423DE2"); } }
        public RequiredPermissionSettings RequiredPermission { get; set; }
        public int UserId { get; set; }
        public override RecordFilterGroup ConvertToRecordFilterGroup(IRDBDataRecordStorageSettingsFilterContext context)
        {
            int? userId;
            Guid posBusinessEntityDefinitionId = new Guid("BACC74BA-AA1B-42CF-9986-152FBE52D71C");

            if (Vanrise.Security.Entities.ContextFactory.GetContext().TryGetLoggedInUserId(out userId))
            {
                SecurityManager securityManager = new SecurityManager();
                if (userId.HasValue && securityManager.IsAllowed(RequiredPermission, userId.Value))
                {
                    GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();

                    List<GenericBusinessEntity> allGenericBusinessEntitiesPOSUsers = genericBusinessEntityManager.GetAllGenericBusinessEntities(new Guid("F5D7FB45-94A8-4FE1-9301-0AC68A95683E"));
                    if (allGenericBusinessEntitiesPOSUsers != null)
                    {
                        IEnumerable<GenericBusinessEntity> loggedInUserPOS = allGenericBusinessEntitiesPOSUsers.Where(x => (int)x.FieldValues.GetRecord("User") == userId.Value);
                        if (loggedInUserPOS != null && loggedInUserPOS.Count() > 0)
                        {
                            List<GenericBusinessEntity> allGenericBusinessEntitiesPOS = genericBusinessEntityManager.GetAllGenericBusinessEntities(posBusinessEntityDefinitionId);
                            if (allGenericBusinessEntitiesPOS != null)
                            {
                                var filteredPOSIds = new List<Object>();
                                foreach (GenericBusinessEntity genericBusinessEntity in allGenericBusinessEntitiesPOS)
                                {
                                    if (loggedInUserPOS.Any(x => x.FieldValues.GetRecord("POS") == genericBusinessEntity.FieldValues.GetRecord("POS")))
                                    {
                                        filteredPOSIds.Add(genericBusinessEntity);
                                    }
                                }
                                var idDataRecordField = _genericBEDefinitionManager.GetIdFieldTypeForGenericBE(posBusinessEntityDefinitionId);
                                RecordFilterGroup recordFilterGroup = new RecordFilterGroup
                                {
                                    LogicalOperator = RecordQueryLogicalOperator.And,
                                    Filters = new List<RecordFilter>()
                                };

                                var objectListRecordFilter = new ObjectListRecordFilter()
                                {
                                    FieldName = "POS",
                                    Values = filteredPOSIds,//filteredPOSIds
                                    CompareOperator = ListRecordFilterOperator.In
                                };
                                RecordFilter convertedRecordFilter = Vanrise.GenericData.Business.Helper.ConvertToRecordFilter("POS", idDataRecordField.Type, objectListRecordFilter);
                                recordFilterGroup.Filters.Add(convertedRecordFilter);
                            }
                        }

                    }
                }
                else return null;
            }



            return null;
        }
    }
}
