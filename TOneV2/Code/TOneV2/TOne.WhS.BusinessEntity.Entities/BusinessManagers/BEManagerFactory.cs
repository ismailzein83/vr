﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Entities
{
    public static class BEManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static BEManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("TOne.WhS.BusinessEntity.Business"));
        }

        public static T GetManager<T>() where T : class, IBEManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
