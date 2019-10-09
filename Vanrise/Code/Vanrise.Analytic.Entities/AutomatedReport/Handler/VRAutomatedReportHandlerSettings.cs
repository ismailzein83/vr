﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class VRAutomatedReportHandlerSettings
    {
        public abstract Guid ConfigId { get; }
        public bool DontExecuteIfEmpty { get; set; }

        public virtual void OnAfterSaveAction(IVRAutomatedReportHandlerSettingsOnAfterSaveActionContext context)
        {

        }
        public abstract void Execute(IVRAutomatedReportHandlerExecuteContext context);

        public virtual void Validate(IVRAutomatedReportHandlerValidateContext context)
        {
            context.Result = QueryHandlerValidatorResult.Successful;
        }

    }
    public interface IVRAutomatedReportHandlerSettingsOnAfterSaveActionContext
    {
        Guid? TaskId { get;}
        long? VRReportGenerationId { get; }
    }
    public class VRAutomatedReportHandlerSettingsOnAfterSaveActionContext : IVRAutomatedReportHandlerSettingsOnAfterSaveActionContext
    {
        public Guid? TaskId { get; set; }
        public long? VRReportGenerationId { get; set; }
    }
    public interface IVRAutomatedReportHandlerExecuteContext
    {
        Guid? TaskId { get; set; }
        IAutomatedReportEvaluatorContext EvaluatorContext { get; set; }
        VRAutomatedReportResolvedDataList GetDataList(Guid vrAutomatedReportQueryId, string listName, bool isFlatFile = false);
        VRAutomatedReportResolvedDataList GetDataList(string queryName, string listName);
        Guid? GetSubTableIdByGroupingFields(List<string> groupingFields, string queryName, string listName);
        VRAutomatedReportResolvedDataFieldValue GetDataField(Guid vrAutomatedReportQueryId, string fieldName);
    }
    public enum QueryHandlerValidatorResult { Successful = 0, Failed = 1}
    public interface IVRAutomatedReportHandlerValidateContext
    {
        List<VRAutomatedReportQuery> Queries { get;}

        QueryHandlerValidatorResult Result { get; set; }
        string ErrorMessage { set; }

    }
}
