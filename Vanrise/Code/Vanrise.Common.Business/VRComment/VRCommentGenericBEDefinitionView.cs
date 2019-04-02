using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.Common.Business
{
    public class VRCommentGenericBEDefinitionView : GenericBEViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("F0FCE857-0119-4895-9459-648E14EFA60A"); }
        }

        public override string RuntimeDirective
        {
            get { return "vr-common-comment-genericbeview-runtime"; }
        }
        public override bool DoesUserHaveAccess(IGenericBEViewDefinitionCheckAccessContext context)
        {
            return base.DoesUserHaveAccess(context);
        }
        public Guid CommentBEDefinitionId { get; set; }
    }
}
