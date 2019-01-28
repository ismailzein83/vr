using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class GenerateFilesActionType
    {
        public abstract Guid ConfigId { get; }
        public abstract void Execute(IGenerateFilesActionTypeContext context);

    }
    public interface IGenerateFilesActionTypeContext
    {
        IVRAutomatedReportHandlerExecuteContext HandlerContext { get; }
        List<GeneratedFileItem> GeneratedFileItems { get; }
    }
    public class GenerateFilesActionTypeContext : IGenerateFilesActionTypeContext
    {
        public IVRAutomatedReportHandlerExecuteContext HandlerContext { get; set; }
        public List<GeneratedFileItem> GeneratedFileItems { get; set; }
    }
    public class GeneratedFileItem
    {
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
}
