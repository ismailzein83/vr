﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface IDynamicPagesDataManager : IDataManager
    {
        List<DynamicPage> GetDynamicPages();
        List<Widget> GetWidgets();
        Boolean SavePage(PageSettings PageSettings);
      
    }
}
