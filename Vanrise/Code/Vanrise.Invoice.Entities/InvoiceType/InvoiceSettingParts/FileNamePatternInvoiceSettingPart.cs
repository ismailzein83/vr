using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class FileNamePatternInvoiceSettingPart : InvoiceSettingPart
    {
        public string FileNamePattern { get; set; }

        public static Guid s_configId = new Guid("01153407-1EC9-4D9D-9722-D4BE5CD419FE");
        public override Guid ConfigId
        {
            get { return s_configId; }
        }
    }
}
