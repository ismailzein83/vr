using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InitialSequenceValueSettingPart : InvoiceSettingPart
    {
        public long InitialValue { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("D75CD1BA-F26B-4E17-BC3C-59996A00495A"); }
        }

    }
}
