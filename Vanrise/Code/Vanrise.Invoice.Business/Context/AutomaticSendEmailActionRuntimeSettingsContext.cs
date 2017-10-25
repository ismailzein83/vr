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
        public Guid AutomaticInvoiceActionId  { get; set; }
        public Entities.Invoice Invoice { get; set; }
        public string ErrorMessage { get; set; }
    }
}
