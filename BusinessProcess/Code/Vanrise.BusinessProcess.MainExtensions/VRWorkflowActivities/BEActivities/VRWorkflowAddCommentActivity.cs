using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.BEActivities
{
    public class VRWorkflowAddCommentActivity : VRWorkflowAddBEActivitySettings
    {
        public VRWorkflowExpression ObjectId { get; set; }
        public VRWorkflowExpression Content { get; set; }
        public VRWorkflowExpression CreatedBy { get; set; }
        public VRWorkflowExpression LastModifiedBy { get; set; }
        public VRWorkflowExpression EntityID { get; set; }
        public VRWorkflowExpression IsSucceeded { get; set; }
        public override string GenerateCode(IVRWorkflowAddBEActivitySettingsGenerateCodeContext context)
        {
            StringBuilder codeBuilder = new StringBuilder();

            codeBuilder.AppendLine("var vrCommentManager = new Vanrise.Common.Business.VRCommentManager();");
            codeBuilder.AppendLine("var vrComment = new Vanrise.Entities.VRComment();");
            codeBuilder.AppendLine($@"vrComment.DefinitionId = new Guid(""{context.EntityDefinitionId}"");");
            if (this.ObjectId != null)
            {
                codeBuilder.AppendLine($@"vrComment.ObjectId = {ObjectId.GetCode(null)});");
            }
            if (this.Content != null)
            {
                codeBuilder.AppendLine($@"vrComment.Content = {Content.GetCode(null)});");
            }
            codeBuilder.AppendLine("var insertOutput = vrCommentManager.AddVRComment(vrComment");

            if (CreatedBy != null || LastModifiedBy != null)
            {
                string createdBy = CreatedBy != null ? CreatedBy.GetCode(null) : "null";
                string lastModifiedBy = LastModifiedBy != null ? LastModifiedBy.GetCode(null) : "null";
                codeBuilder.AppendLine($@",{createdBy},{lastModifiedBy}");
            }
            codeBuilder.AppendLine(");");

            if (this.IsSucceeded != null)
            {
                codeBuilder.AppendLine($"{this.IsSucceeded.GetCode(null)} = (insertOutput.Result == Vanrise.Entities.InsertOperationResult.Succeeded);");
            }

            if (this.EntityID != null)
            {
                codeBuilder.AppendLine(@"if(insertOutput.Result == Vanrise.Entities.InsertOperationResult.Succeeded) {");
                codeBuilder.AppendLine($@"{this.EntityID.GetCode(null)} = insertOutput.InsertedObject.VRCommentId;");
                codeBuilder.AppendLine(@"}");
            }

            return codeBuilder.ToString();
        }

    }
}
