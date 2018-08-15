using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers
{
    public class GeneratedFileItem
    {
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
    public class GenerateFilesHandler : VRAutomatedReportHandlerSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("F139CA56-D602-4D91-9FC2-A14418D9831E"); }
        }
        public override void Execute(IVRAutomatedReportHandlerExecuteContext context)
        {
            if (this.AttachementGenerators != null && this.AttachementGenerators.Count > 0 && ActionType != null)
            {
                List<GeneratedFileItem> generatedFileItems = new List<GeneratedFileItem>();
                int totalAttachments = this.AttachementGenerators.Count;
                int attachmentsDone = 0;
                int attachmentsLeft = totalAttachments;
                if (context.EvaluatorContext != null)
                {
                    if (totalAttachments == 1)
                        context.EvaluatorContext.WriteInformationBusinessTrackingMsg("The total number of attachments to generate is 1 attachment.");
                    else
                        context.EvaluatorContext.WriteInformationBusinessTrackingMsg("The total number of attachments to generate is {0} attachments.", totalAttachments);
                }
               
                AdvancedExcelFileGeneratorManager fileGeneratorManager = new AdvancedExcelFileGeneratorManager();

                foreach (var generator in this.AttachementGenerators)
                {
                    generator.Settings.ThrowIfNull("attachement.Settings");

                    VRAutomatedReportFileGeneratorGenerateFileContext generateFileContext = new VRAutomatedReportFileGeneratorGenerateFileContext
                    {
                        HandlerContext = context
                    };
                    var generatedFileOutput = fileGeneratorManager.GenerateFileOutput(generator, generateFileContext);
                    if (generatedFileOutput != null)
                    {
                        if (context.EvaluatorContext != null)
                        {
                            attachmentsDone++;
                            attachmentsLeft = totalAttachments - attachmentsDone;
                            context.EvaluatorContext.WriteInformationBusinessTrackingMsg("The number of attachments left is {0} out of {1} attachments.", attachmentsLeft, totalAttachments);
                        }
                        generatedFileOutput.GeneratedFile.ThrowIfNull("generatedFileOutput.GeneratedFile");

                        generatedFileItems.Add(new GeneratedFileItem
                        {
                            FileContent = generatedFileOutput.GeneratedFile.FileContent,
                            FileName = generatedFileOutput.FileName
                        });
                    }
                }
                ActionType.Execute(new GenerateFilesActionTypeContext { GeneratedFileItems = generatedFileItems, HandlerContext = context });
            }
           
        }
        public override void OnAfterSaveAction(IVRAutomatedReportHandlerSettingsOnAfterSaveActionContext context)
        {
            if (AttachementGenerators != null)
            {
                foreach (var attachementGenerator in AttachementGenerators)
                {
                    if (attachementGenerator.Settings != null)
                        attachementGenerator.Settings.OnAfterSaveAction(new VRAutomatedReportFileGeneratorOnAfterSaveActionContext { TaskId = context.TaskId, VRReportGenerationId = context.VRReportGenerationId });
                }
            }
        }
        public GenerateFilesActionType ActionType { get; set; }
        public List<VRAutomatedReportFileGenerator> AttachementGenerators { get; set; }
        public override void Validate(IVRAutomatedReportHandlerValidateContext context)
        {
            this.AttachementGenerators.ThrowIfNull("No attachment generators were added.");
            foreach (var generator in this.AttachementGenerators)
            {
                generator.Settings.ThrowIfNull("generator.Settings");
                generator.Settings.Validate(context);
                if (context.Result == QueryHandlerValidatorResult.Failed)
                    break;
            }
        }

    }
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

    public class FTPActionType : GenerateFilesActionType
    {
        public override Guid ConfigId
        {
            get { return new Guid("F38308EF-433A-48C6-891B-34847929FD5A"); }
        }
        public string Subdirectory { get; set; }
        public FTPCommunicatorSettings FTPCommunicatorSettings { get; set; }

        public override void Execute(IGenerateFilesActionTypeContext context)
        {
            if (context.GeneratedFileItems != null && context.GeneratedFileItems.Count > 0 && this.FTPCommunicatorSettings != null)
            {
                AdvancedExcelFileGeneratorManager fileGeneratorManager = new AdvancedExcelFileGeneratorManager();
                int totalFiles = context.GeneratedFileItems.Count;
                int filesDone = 0;
                int filesLeft = totalFiles;
                using (FTPCommunicator ftpCommunicator = new FTPCommunicator(this.FTPCommunicatorSettings))
                {
                    foreach (var generator in context.GeneratedFileItems)
                    {
                        MemoryStream stream = new MemoryStream(generator.FileContent);
                        string errorMessage = null;

                        if (!ftpCommunicator.TryWriteFile(stream, generator.FileName, this.Subdirectory, out errorMessage))
                        {
                            if (context.HandlerContext.EvaluatorContext != null)
                                context.HandlerContext.EvaluatorContext.WriteErrorBusinessTrackingMsg(errorMessage);
                            else
                                throw new Exception(errorMessage);
                        }
                        else
                        {
                            if (context.HandlerContext.EvaluatorContext != null)
                            {
                                filesDone++;
                                filesLeft = totalFiles - filesDone;
                                context.HandlerContext.EvaluatorContext.WriteInformationBusinessTrackingMsg("The number of files left to generate and transfer is {0} out of {1} files.", filesLeft, totalFiles);
                            }
                        }
                    }
                }
            }
        }


    }
    public class SendEmailActionType : GenerateFilesActionType
    {
        public override Guid ConfigId
        {
            get { return new Guid("D7D6D580-40BD-42C6-ABBB-7FA6B60A5462"); }
        }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public override void Execute(IGenerateFilesActionTypeContext context)
        {
            if (context.GeneratedFileItems != null && context.GeneratedFileItems.Count > 0)
            {
                List<VRMailAttachement> attachements = new List<VRMailAttachement>();
                foreach (var generator in context.GeneratedFileItems)
                {
                    VRMailAttachmentExcel excelAttachment = new VRMailAttachmentExcel()
                    {
                        Name = generator.FileName,
                        Content = generator.FileContent
                    };
                    attachements.Add(excelAttachment);
                }
                new VRMailManager().SendMail(this.To, null, null, this.Subject, this.Body, attachements);
                if (context.HandlerContext.EvaluatorContext != null)
                    context.HandlerContext.EvaluatorContext.WriteInformationBusinessTrackingMsg("An e-mail has been sent to {0}.", this.To);
            }
        }
    }
}
