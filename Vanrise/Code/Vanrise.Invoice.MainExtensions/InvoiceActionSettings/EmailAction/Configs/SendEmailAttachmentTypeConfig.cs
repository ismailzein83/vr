using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class SendEmailAttachmentTypeConfig:ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_InvoiceActions_SendEmailAttachmentTypeConfig";
        public string Editor { get; set; }
    }
}
