using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.CP;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Entities.CP.Processing
{
    public class ZoneToProcess : IRuleTarget
    {
        public string ZoneName { get; set; }

        public List<CodeToAdd> CodesToAdd { get; set; }

        public List<CodeToMove> CodesToMove { get; set; }

        public List<CodeToClose> CodesToClose { get; set; }
        public bool IsExcluded { get; set; }

        public object Key
        {
            get { return this.ZoneName; }
        }

        public void SetExcluded()
        {
            this.IsExcluded = true;
            foreach (CodeToAdd codeToAdd in CodesToAdd)
                codeToAdd.SetExcluded();
            foreach (CodeToMove codeToMove in CodesToMove)
                codeToMove.SetExcluded();
            foreach (CodeToClose codeToClose in CodesToClose)
                codeToClose.SetExcluded();
        }

        public string TargetType
        {
            get { return "Zone"; }
        }
    }
}
