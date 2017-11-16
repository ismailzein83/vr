using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business.Context
{
    public class AutomaticActionRuntimeSettingsContext : IAutomaticActionRuntimeSettingsContext
    {
        public Entities.Invoice Invoice { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsErrorOccured { get; set; }
        public AutomaticInvoiceActionSettings DefinitionSettings { get; set; }
    }
}
