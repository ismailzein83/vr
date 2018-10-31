using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
    public interface IPageRunTimeDataManager : IDataManager
    {

        bool ArePageRunTimesUpdated(ref object updateHandle);
        bool Insert(PageRunTime pageRunTime, out int insertedId);

        bool Update(PageRunTime pageRunTime);

        List<PageRunTime> GetPageRunTimes();

      
      

    }
}
