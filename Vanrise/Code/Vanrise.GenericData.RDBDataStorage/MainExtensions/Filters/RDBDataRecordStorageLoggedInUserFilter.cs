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
        GenericBusinessEntityDefinitionManager genericBEDefinitionManager = new GenericBusinessEntityDefinitionManager();
        public override Guid ConfigId { get { return new Guid("CF3123E6-1A84-4A8A-873B-DF7DBF423DE2"); } }
        public RequiredPermissionSettings RequiredPermission { get; set; }
        public int UserId { get; set; }
        public override RecordFilterGroup ConvertToRecordFilterGroup(IRDBDataRecordStorageSettingsFilterContext context)
        {
            int? userId;
            Guid posBusinessEntityDefinitionId = new Guid("BACC74BA-AA1B-42CF-9986-152FBE52D71C");
            Guid stockItemBEDefinitionId = new Guid("EEED28F1-6654-4801-BE39-D4D397CFEE12");
            if (Vanrise.Security.Entities.ContextFactory.GetContext().TryGetLoggedInUserId(out userId))
            {
                SecurityManager securityManager = new SecurityManager();
                if (userId.HasValue)
                {
                    if (RequiredPermission != null && RequiredPermission.Entries != null && RequiredPermission.Entries.Count() > 0 && securityManager.IsAllowed(RequiredPermission, userId.Value))
                        return null;
                    GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();

                    List<GenericBusinessEntity> allGenericBusinessEntitiesPOSUsers = genericBusinessEntityManager.GetAllGenericBusinessEntities(new Guid("F5D7FB45-94A8-4FE1-9301-0AC68A95683E"));
                    if (allGenericBusinessEntitiesPOSUsers != null)
                    {
                        List<GenericBusinessEntity> loggedInUserPOS = new List<GenericBusinessEntity>();
                        foreach (var be in allGenericBusinessEntitiesPOSUsers)
                        {
                            if ((int)be.FieldValues.GetRecord("User") == userId.Value)
                            {
                                loggedInUserPOS.Add(be);
                            }
                        }

                        if (loggedInUserPOS != null && loggedInUserPOS.Count() > 0)
                        {
                            List<GenericBusinessEntity> allGenericBusinessEntitiesPOS = genericBusinessEntityManager.GetAllGenericBusinessEntities(posBusinessEntityDefinitionId);
                            if (allGenericBusinessEntitiesPOS != null)
                            {
                                var filteredPOSIds = new List<Object>();
                                foreach (GenericBusinessEntity genericBusinessEntity in allGenericBusinessEntitiesPOS)
                                {
                                    foreach (var userPOS in loggedInUserPOS)
                                    {
                                        if ((Guid)userPOS.FieldValues.GetRecord("POS") == (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"))
                                            filteredPOSIds.Add(genericBusinessEntity.FieldValues.GetRecord("ID"));
                                    }
                                }
                                var idDataRecordFields = genericBEDefinitionManager.GetDataRecordTypeFieldsByBEDefinitionId(stockItemBEDefinitionId);
                                idDataRecordFields.ThrowIfNull("idDataRecordField");
                                var idDataRecordField = idDataRecordFields.GetRecord("POS");
                                idDataRecordField.ThrowIfNull("idDataRecordField");


                                RecordFilterGroup recordFilterGroup = new RecordFilterGroup
                                {
                                    LogicalOperator = RecordQueryLogicalOperator.And,
                                    Filters = new List<RecordFilter>()
                                };

                                var objectListRecordFilter = new ObjectListRecordFilter()
                                {
                                    FieldName = "ID",
                                    Values = filteredPOSIds,
                                    CompareOperator = ListRecordFilterOperator.In
                                };
                                RecordFilter convertedRecordFilter = Vanrise.GenericData.Business.Helper.ConvertToRecordFilter("POS", idDataRecordField.Type, objectListRecordFilter);
                                recordFilterGroup.Filters.Add(convertedRecordFilter);
                                return recordFilterGroup;
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
