using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class StaticFilterDefinitionSettings : GenericBEFilterDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("FDDFA89B-B871-4864-9BF3-590262EC68F2"); }
        }

        public override string RuntimeEditor
        {
            get { throw new NotImplementedException(); }
        }
        public string DirectiveName { get; set; }
    }
}
