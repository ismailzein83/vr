using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Retail.BusinessEntity.MainExtensions.AccountActions
{
    public class SendEmailAction : VRAction
    {
        public override Guid ConfigId
        {
            get { return new Guid("BE74A60E-D312-4B4F-BD76-5B7BE81ABE62"); }
        }

        public override void Execute(IVRActionExecutionContext context)
        {
            string path =string.Format( @"C:\SendEmail_{0}.txt", Guid.NewGuid().ToString("N"));
            File.Create(path );
   
        }
    }


}
