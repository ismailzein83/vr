﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Business
{
    public interface IBusinessEntityManager
    {
        string GetEntityDescription(object entityId);
    }
}
