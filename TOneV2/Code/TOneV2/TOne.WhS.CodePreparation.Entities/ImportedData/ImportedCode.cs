using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Entities
{
    public class ImportedCode : IRuleTarget
    {
        public string Code { get; set; }

        public string ZoneName { get; set; }

        public ImportType? Status { get; set; }



        public object Key
        {
            get { return this.ZoneName != string.Empty ? this.ZoneName : this.Code; }
        }

        public void SetExcluded()
        {
            throw new NotImplementedException();
        }

        public string TargetType
        {
            get { return "Code"; }
        }
    }
}
