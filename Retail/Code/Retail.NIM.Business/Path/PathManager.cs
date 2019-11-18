using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.NIM.Business
{
    public class PathManager
    {
        static Guid s_pathBEDefinitionId = new Guid("95DCF8AF-2273-4356-81E7-081034CCD75B");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
        static string s_idFieldName = "ID";
        static string s_nameFieldName = "Name";
        static string s_statusIdFieldName = "Status";

        #region Path Status
        public static Guid s_readyPathStatusDefinitionId = new Guid("a7815af4-e6d9-4dd0-bd1a-3f4b8b7b72d0");
        public static Guid s_draftPathStatusDefinitionId = new Guid("d5618e4c-50f5-41bd-9b2e-c4e2d70d6715");
        #endregion

        #region Public Methods
        public PathOutput CreatePath(PathInput pathInput)
        {
            var insertedEntity = _genericBusinessEntityManager.AddGenericBusinessEntity(new GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = s_pathBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { s_nameFieldName, pathInput.Name }, { s_statusIdFieldName, s_draftPathStatusDefinitionId } }
            });

            if (insertedEntity.Result == InsertOperationResult.Failed)
                return null;
            return new PathOutput()
            {
                PathId = (long)insertedEntity.InsertedObject.FieldValues.GetRecord(s_idFieldName).Value
            };
        }
        public SetPathReadyOutput SetPathReady(SetPathReadyInput input)
        {
            var updatedEntity = _genericBusinessEntityManager.UpdateGenericBusinessEntity(new GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = s_pathBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { s_statusIdFieldName, s_readyPathStatusDefinitionId } },
                GenericBusinessEntityId = input.PathId,
                FilterGroup = new RecordFilterGroup
                {
                    Filters = new List<RecordFilter>
                    {
                        new ObjectListRecordFilter
                        {
                              FieldName = s_statusIdFieldName,
                              Values = new List<object> { s_draftPathStatusDefinitionId }
                        }
                   }
                }
            });

            return new SetPathReadyOutput
            {
                IsSucceeded = (updatedEntity.Result == UpdateOperationResult.Succeeded)
            };
        }

        #endregion
 
    }
   
}
