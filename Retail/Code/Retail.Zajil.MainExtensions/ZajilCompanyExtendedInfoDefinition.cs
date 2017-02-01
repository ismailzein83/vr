using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Zajil.MainExtensions
{
    public class ZajilCompanyExtendedInfoDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("F6630722-4E85-4DF2-915F-F9942074743C"); } }
    }
}
