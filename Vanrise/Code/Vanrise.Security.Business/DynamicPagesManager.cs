using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class DynamicPagesManager
    {
        public List<DynamicPage> GetDynamicPages()
        {
            IDynamicPagesDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IDynamicPagesDataManager>();
            return dataManager.GetDynamicPages();
        }
        public List<Widget> GetWidgets()
        {
            IDynamicPagesDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IDynamicPagesDataManager>();
            return dataManager.GetWidgets();
        }
    }
}                               
