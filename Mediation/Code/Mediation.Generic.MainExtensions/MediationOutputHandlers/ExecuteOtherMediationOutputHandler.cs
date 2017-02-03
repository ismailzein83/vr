using Mediation.Generic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.QueueActivators;
using Mediation.Generic.Business;

namespace Mediation.Generic.MainExtensions.MediationOutputHandlers
{
    public class ExecuteOtherMediationOutputHandler : MediationOutputHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("F1D57186-49CE-4BF9-B4B6-46DDCE93E9EC"); }
        }

        public int MediationDefinitionId { get; set; }

        public override void Execute(IMediationOutputHandlerContext context)
        {
            MediationRecordsManager mediationRecordsManager = new MediationRecordsManager();
            MediationDefinitionManager mediationManager = new MediationDefinitionManager();
            MediationDefinition mediationDefinition = mediationManager.GetMediationDefinition(this.MediationDefinitionId);
            RecordFilterManager filterManager = new RecordFilterManager();

            List<MediationRecord> mediationRecords = new List<MediationRecord>();
            context.DoWhilePreviousRunning(() =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = context.InputQueue.TryDequeue((preparedCdrBatch) =>
                    {
                        foreach (var cdr in preparedCdrBatch.Cdrs)
                        {
                            MediationRecord mediationRecord = new MediationRecord();
                            DataRecordFilterGenericFieldMatchContext dataRecordFilterContext = new DataRecordFilterGenericFieldMatchContext(cdr, mediationDefinition.ParsedRecordTypeId);
                            mediationRecord.SessionId = GetPropertyValue(cdr, mediationDefinition.ParsedRecordIdentificationSetting.SessionIdField) as string;
                            mediationRecord.EventTime = (DateTime)GetPropertyValue(cdr, mediationDefinition.ParsedRecordIdentificationSetting.EventTimeField);
                            foreach (var statusMapping in mediationDefinition.ParsedRecordIdentificationSetting.StatusMappings)
                            {
                                if (filterManager.IsFilterGroupMatch(statusMapping.FilterGroup, dataRecordFilterContext))
                                {
                                    mediationRecord.EventStatus = statusMapping.Status;
                                    break;
                                }
                            }
                            mediationRecord.EventDetails = cdr;
                            mediationRecord.MediationDefinitionId = mediationDefinition.MediationDefinitionId;
                            mediationRecords.Add(mediationRecord);
                        }
                        mediationRecordsManager.SaveMediationRecordsToDB(mediationRecords);
                    });
                } while (!context.ShouldStop() && hasItem);
            });           
            
        }

        object GetPropertyValue(object batchRecord, string propertyName)
        {
            var reader = Vanrise.Common.Utilities.GetPropValueReader(propertyName);
            return reader.GetPropertyValue(batchRecord);
        }
    }
}
