using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Entities
{
    public class NewZone : IRuleTarget
    {
        public string Name { get; set; }
        public int CountryId { get; set; }
        public bool hasChanges { get; set; }

        public object Key
        {
            get { return this.Name; }
        }

        public void SetExcluded()
        {
            throw new NotImplementedException();
        }

        public string TargetType
        {
            get { return "Zone"; }
        }
    }
}
