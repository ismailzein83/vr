﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public interface IPartnerManager
    {
        string GetPartnerName(IPartnerManagerContext context);
        dynamic GetPartnerInfo(IPartnerManagerInfoContext context);
    }
    public interface IPartnerManagerContext
    {

        string  PartnerId { get;}
    }
}
