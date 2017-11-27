using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.NumberingPlan.Entities
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

        public string TargetType
        {
            get { return "Code"; }
        }

       
    }

    public enum CodeChangeType
    {

        [Description("Not Changed")]
        NotChanged = 0,

        [Description("New")]
        New = 1,

        [Description("Deleted")]
        Deleted = 2,

        [Description("Moved")]
        Moved = 3,

        [Description("Pending Effective")]
        PendingEffective = 4,

        [Description("Pending Closed")]
        PendingClosed = 5,

    }

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
