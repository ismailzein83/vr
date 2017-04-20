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
            get { return new Guid("F0DEC732-929C-4F75-AA35-9E19298D3092"); }
        }

        public GenericLKUPDefinitionExtendedSettings ExtendedSettings { get; set; }
    }
}
