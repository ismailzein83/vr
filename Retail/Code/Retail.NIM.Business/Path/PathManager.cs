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
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
    
        #region Public Methods
        public PathOutput CreatePath(PathInput pathInput)
        {
            var insertedEntity = _genericBusinessEntityManager.AddGenericBusinessEntity(new GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = StaticBEDefinitionIDs.PathBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { "Name", pathInput.Name }, { "Status", StaticBEDefinitionIDs.DraftPathStatusDefinitionId } }
            });

            if (insertedEntity.Result == InsertOperationResult.Failed)
                return null;
            return new PathOutput()
            {
                PathId = (long)insertedEntity.InsertedObject.FieldValues.GetRecord("ID").Value
            };
        }

        public PathConnectionOutput AddConnectionToPath(PathConnectionInput pathConnectionInput)
        {
            var insertedEntity = _genericBusinessEntityManager.AddGenericBusinessEntity(new GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = StaticBEDefinitionIDs.PathConnectionBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { "Path", pathConnectionInput.PathId }, { "Connection", pathConnectionInput.ConnectionId } }
            });


            if (insertedEntity.Result == InsertOperationResult.Failed)
                return null;

            return new PathConnectionOutput
            {
                PathConnectionId = (long)insertedEntity.InsertedObject.FieldValues.GetRecord("ID").Value
            };
        }

        public SetPathReadyOutput SetPathReady(SetPathReadyInput input)
        {
            var updatedEntity = _genericBusinessEntityManager.UpdateGenericBusinessEntity(new GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = StaticBEDefinitionIDs.PathBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { "Status", StaticBEDefinitionIDs.ReadyPathStatusDefinitionId } },
                GenericBusinessEntityId = input.PathId,
                FilterGroup = new RecordFilterGroup
                {
                    Filters = new List<RecordFilter>
                    {
                        new ObjectListRecordFilter
                        {
                              FieldName = "Status",
                              Values = new List<object> { StaticBEDefinitionIDs.DraftPathStatusDefinitionId }
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
