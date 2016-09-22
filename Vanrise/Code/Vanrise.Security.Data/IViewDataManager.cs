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
        bool AddView(View view);
        bool UpdateView(View view);
        bool DeleteView(Guid viewId);
        bool UpdateViewRank(Guid viewId, Guid moduleId, int rank);

        bool AreViewsUpdated(ref object updateHandle);
    }
}
