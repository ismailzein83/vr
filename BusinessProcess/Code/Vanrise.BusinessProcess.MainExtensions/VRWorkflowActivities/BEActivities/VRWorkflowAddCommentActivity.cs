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
        public VRWorkflowExpression UserId { get; set; }
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
            if (UserId != null)
            {
                string userId = UserId.GetCode(null);
                codeBuilder.AppendLine($@",{userId}");
            }
            codeBuilder.AppendLine(");");

            if (this.IsSucceeded != null)
            {
                codeBuilder.AppendLine($"{this.IsSucceeded.GetCode(null)} = (insertOutput.Result == Vanrise.Entities.InsertOperationResult.Succeeded);");
            }
            return codeBuilder.ToString();
        }

    }
}
