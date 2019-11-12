using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnAfterSaveHandlers
{
    public class SaveHistoryGenericBEAfterSaveHandler : GenericBEOnAfterSaveHandler
    {
        static string BETFieldName = "BET";
        static string EETFieldName = "EET";
        public override Guid ConfigId
        {
            get { return new Guid("79EA016F-3563-41BA-8928-FE0FEB4390AA"); }
        }
        public Guid HistoryBEDefintionID { get; set; }
        public string ParentFieldName { get; set; }
        public override void Execute(IGenericBEOnAfterSaveHandlerContext context)
        {
            DateTime bet = DateTime.Now;

            GenericBusinessEntityDefinitionManager genericBusinessEntityDefinitionManager = new GenericBusinessEntityDefinitionManager();
            var parentBEIdField = genericBusinessEntityDefinitionManager.GetIdFieldTypeForGenericBE(context.BusinessEntityDefinitionId);
            var historyBEIdField = genericBusinessEntityDefinitionManager.GetIdFieldTypeForGenericBE(HistoryBEDefintionID);

            var historyDataRecordFields = genericBusinessEntityDefinitionManager.GetDataRecordTypeFieldsByBEDefinitionId(HistoryBEDefintionID);
            var historyFieldValues = new Dictionary<string, object>();

            var betField = historyDataRecordFields.GetRecord(BETFieldName);
            if (betField != null)
                historyFieldValues.Add(betField.Name, bet);

            historyFieldValues.Add(ParentFieldName, context.NewEntity.FieldValues.GetRecord(parentBEIdField.Name));

            if (historyDataRecordFields != null && historyDataRecordFields.Count > 0)
            {
                foreach (var field in historyDataRecordFields)
                {
                    List<string> fieldsToExcludeFromMapping = new List<string> { historyBEIdField.Name, ParentFieldName, BETFieldName, EETFieldName };
                    var fieldName = field.Value.Name;

                    if (!fieldsToExcludeFromMapping.Contains(fieldName))
                        historyFieldValues.Add(fieldName, context.NewEntity.FieldValues.GetRecord(fieldName));
                }
            }

            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            if (context.OperationType == HandlerOperationType.Update)
            {
                var historyEntitiesToSetEET = GetHistoryEntitiesToSetEET(context.BusinessEntityDefinitionId, bet, historyBEIdField.Name);
                if (historyEntitiesToSetEET != null && historyEntitiesToSetEET.Count > 0)
                {
                    foreach (var entity in historyEntitiesToSetEET)
                    {
                        genericBusinessEntityManager.UpdateGenericBusinessEntity(entity);
                    }
                }
            }

            var historyGenericBEToAdd = new GenericBusinessEntityToAdd()
            {
                BusinessEntityDefinitionId = HistoryBEDefintionID,
                FieldValues = historyFieldValues
            };

            genericBusinessEntityManager.AddGenericBusinessEntity(historyGenericBEToAdd);
        }
        private List<GenericBusinessEntityToUpdate> GetHistoryEntitiesToSetEET(object parentBEID, DateTime effectiveTime, string historyBEIdFieldName)
        {
            var filterGroup = new RecordFilterGroup { Filters = new List<RecordFilter>() };

            filterGroup.Filters.Add(new ObjectListRecordFilter
            {
                FieldName = ParentFieldName,
                Values = new List<object> { parentBEID }
            });

            var orFilterGroup = new RecordFilterGroup
            {
                Filters = new List<RecordFilter>(),
                LogicalOperator = RecordQueryLogicalOperator.Or
            };
            filterGroup.Filters.Add(orFilterGroup);

            orFilterGroup.Filters.Add(new EmptyRecordFilter { FieldName = EETFieldName });
            orFilterGroup.Filters.Add(new DateTimeRecordFilter
            {
                FieldName = EETFieldName,
                CompareOperator = DateTimeRecordFilterOperator.Greater,
                Value = effectiveTime
            });

            var historyEntities = new GenericBusinessEntityManager().GetAllGenericBusinessEntities(HistoryBEDefintionID, new List<string> { historyBEIdFieldName, ParentFieldName, BETFieldName, EETFieldName }, filterGroup);
            var historyEntitiesToUpdate = new List<GenericBusinessEntityToUpdate>();

            if (historyEntities != null)
            {
                foreach (var historyEntity in historyEntities)
                {
                    DateTime? bet = (DateTime?)historyEntity.FieldValues[BETFieldName];
                    DateTime eetToSet;
                    if (bet.HasValue && bet.Value > effectiveTime)
                        eetToSet = bet.Value;
                    else
                        eetToSet = effectiveTime;

                    var historyEntityToUpdate = new GenericBusinessEntityToUpdate
                    {
                        BusinessEntityDefinitionId = HistoryBEDefintionID,
                        GenericBusinessEntityId = historyEntity.FieldValues[historyBEIdFieldName],
                        FieldValues = new Dictionary<string, object>()
                    };

                    historyEntityToUpdate.FieldValues.Add(EETFieldName, eetToSet);
                    historyEntitiesToUpdate.Add(historyEntityToUpdate);
                }
            }
            return historyEntitiesToUpdate;
        }
    }
}
