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
        bool AddView(View view, out int insertedId);
        bool UpdateView(View view);
        bool DeleteView(int viewId);
        View GetView(int viewId);
        Vanrise.Entities.BigResult<View> GetFilteredDynamicViews(Vanrise.Entities.DataRetrievalInput<string> filter);
        bool UpdateViewRank(int viewId,int rank);
    }
}
