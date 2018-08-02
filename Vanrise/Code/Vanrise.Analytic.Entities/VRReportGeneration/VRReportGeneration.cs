using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public class VRReportGeneration
    {
        public long ReportId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public VRReportGenerationSettings Settings { get; set; }
        public DateTime CreatedTime { get; set; }
        public int CreatedBy { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public AccessLevel AccessLevel { get; set; }
    }
    public class VRReportGenerationSettings
    {
        public List<VRAutomatedReportQuery> Queries { get; set; }
        public VRReportGenerationAction ReportAction { get; set; }
        public VRReportGenerationFilter Filter { get; set; }
    }
    public abstract class VRReportGenerationFilter
    {
        public abstract Guid ConfigId { get; }
        public abstract string RuntimeEditor { get; }
    }
    public abstract class VRReportGenerationRuntimeFilter
    {
        public abstract VRReportGenerationFilterContent GetFilterContent(IVRReportGenerationRuntimeFilterContext context);
    }
    public interface IVRReportGenerationRuntimeFilterContext
    {
        VRReportGenerationFilter FilterDefinition { get; }
    }
    public class VRReportGenerationFilterContent
    {
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public RecordFilterGroup FilterGroup { get; set; }
    }

    public abstract class VRReportGenerationAction
    {

        public virtual void OnAfterSaveAction(IVRReportGenerationActionOnAfterSaveActionContext context)
        {

        }
        public abstract string ActionTypeName { get; }
        public abstract Guid ConfigId { get; }
    }
    public interface IVRReportGenerationActionOnAfterSaveActionContext
    {
        long VRReportGenerationId { get; }
    }
    public class VRReportGenerationActionOnAfterSaveActionContext : IVRReportGenerationActionOnAfterSaveActionContext
    {
        public long VRReportGenerationId { get; set; }
    }
    public enum AccessLevel
    {
        [Description("Public")]
        Public = 0,
        [Description("Private")]
        Private = 1
    }
}
