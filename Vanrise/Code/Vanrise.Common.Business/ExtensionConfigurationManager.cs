﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class ExtensionConfigurationManager
    {
        public IEnumerable<T> GetExtensionConfigurations<T>(string type) where T : ExtensionConfiguration
        {
            throw new NotImplementedException();
        }

      
    }
}
