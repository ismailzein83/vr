using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class GenericLKUPBEDefinitionSettings : Vanrise.GenericData.Entities.BusinessEntityDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public GenericLKUPDefinitionExtendedSettings ExtendedSettings { get; set; }
    }
}
