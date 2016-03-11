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
        bool AddView(View view, out int insertedId);
        bool UpdateView(View view);
        bool DeleteView(int viewId);
        bool UpdateViewRank(int viewId,int moduleId,int rank);

        bool AreViewsUpdated(ref object updateHandle);
    }
}
