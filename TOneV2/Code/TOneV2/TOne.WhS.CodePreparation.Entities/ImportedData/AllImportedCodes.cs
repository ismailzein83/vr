using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Entities
{

    public class AllImportedCodes : IRuleTarget
    {
        public IEnumerable<ImportedCode> ImportedCodes { get; set; }

        public object Key
        {
            get { return null; }
        }
        public string TargetType
        {
            get { return "AllImportedCodes"; }
        }
    }
}