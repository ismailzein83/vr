﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace TOne.WhS.AccountBalance.Business
{
    public class SendSupplierEmailBalAlertAction : VRAction
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public Guid AccountMailTemplateId { get; set; }

        public Guid ProfileMailTemplateId { get; set; }
    }
}
