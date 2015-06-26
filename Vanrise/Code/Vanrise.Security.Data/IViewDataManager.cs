using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface IViewDataManager : IDataManager
    {
        List<View> GetViews();
        List<View> GetDynamicPages();
        bool SaveView(View view, out int insertedId);
        View GetView(int viewId);
    }
}
