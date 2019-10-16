﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.BP.Arguments;
using Vanrise.Analytic.Entities;
using Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers
{
    public class SendEmailHandler : VRAutomatedReportHandlerSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("C05375FD-4C3A-44B2-ACEE-A0EDEE56B488"); }
        }

        public string To { get; set; }

        public string CC { get; set; }

        //public string BCC { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public List<VRAutomatedReportFileGenerator> AttachementGenerators { get; set; }
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
        public override void Execute(IVRAutomatedReportHandlerExecuteContext context)
        {
            if (this.AttachementGenerators != null && this.AttachementGenerators.Count > 0)
            {
                List<VRMailAttachement> attachements = new List<VRMailAttachement>();
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

                foreach (var generator in AttachementGenerators)
                {
                    generator.Settings.ThrowIfNull("attachement.Settings");

                    VRAutomatedReportFileGeneratorGenerateFileContext generateFileContext = new VRAutomatedReportFileGeneratorGenerateFileContext
                    {
                        DontExecuteIfEmpty = DontExecuteIfEmpty,
                        HandlerContext = context
                    };

                    var generatedFileOutput = fileGeneratorManager.GenerateFileOutput(generator, generateFileContext);
                    if (generatedFileOutput != null && (!generatedFileOutput.GeneratedFile.FileIsEmpty || !DontExecuteIfEmpty))
                    {
                        if (context.EvaluatorContext != null)
                        {
                            attachmentsDone++;
                            attachmentsLeft = totalAttachments - attachmentsDone;
                            context.EvaluatorContext.WriteInformationBusinessTrackingMsg("The number of attachments to generate left is {0} out of {1} attachments.", attachmentsLeft, totalAttachments);
                        }
                        generatedFileOutput.GeneratedFile.ThrowIfNull("generatedFileOutput.GeneratedFile");
                        VRMailAttachmentGeneral attachment = new VRMailAttachmentGeneral()
                        {
                            Name = generatedFileOutput.FileName,
                            Content = generatedFileOutput.GeneratedFile.FileContent
                        };
                        attachements.Add(attachment);
                    }
                }
                if (attachements.Count > 0)
                {
                    new VRMailManager().SendMail(this.To, this.CC, null, this.Subject, this.Body, attachements);
                    if (context.EvaluatorContext != null)
                        context.EvaluatorContext.WriteInformationBusinessTrackingMsg("An e-mail has been sent to {0}.", this.To);
                }
            }
        }

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
}
