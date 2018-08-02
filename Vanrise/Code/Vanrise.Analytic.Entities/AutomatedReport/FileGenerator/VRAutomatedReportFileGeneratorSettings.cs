using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class VRAutomatedReportFileGeneratorSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual void Validate(IVRAutomatedReportHandlerValidateContext context)
        {
            
        }
        public virtual void OnAfterSaveAction(IVRAutomatedReportFileGeneratorOnAfterSaveActionContext context)
        {

        }
        public abstract VRAutomatedReportGeneratedFile GenerateFile(IVRAutomatedReportFileGeneratorGenerateFileContext context);
    }
    public interface IVRAutomatedReportFileGeneratorOnAfterSaveActionContext
    {
        Guid? TaskId { get; }
        long? VRReportGenerationId { get; }
    }
    public class VRAutomatedReportFileGeneratorOnAfterSaveActionContext : IVRAutomatedReportFileGeneratorOnAfterSaveActionContext
    {

        public Guid? TaskId { get; set; }

        public long? VRReportGenerationId { get; set; }
    }

    public interface IVRAutomatedReportFileGeneratorGenerateFileContext
    {
        IVRAutomatedReportHandlerExecuteContext HandlerContext { get; }
    }
}
