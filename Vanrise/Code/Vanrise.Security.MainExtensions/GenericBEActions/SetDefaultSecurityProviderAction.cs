using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.Security.MainExtensions.GenericBEActions
{
    public class SetDefaultSecurityProviderAction : GenericBEActionSettings
    {
        public override Guid ConfigId { get { return new Guid("D44A8DED-2E6B-4226-99C7-BAA6895BECBE"); } }

        public override string ActionTypeName { get { return "SetDefaultSecurityProviderAction"; } }

        public override bool DoesUserHaveAccess(IGenericBEActionDefinitionCheckAccessContext context)
        {
            return new GenericBusinessEntityManager().DoesUserHaveEditAccess(context.UserId, context.BusinessEntityDefinitionId);
        }
    }
}
