using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
    public interface IDemoCityDataManager : IDataManager
    {
        bool AreDemoCitiesUpdated(ref object updateHandle);

        List<DemoCity> GetDemoCities();

        bool Insert(DemoCity city, out int insertedId);

        bool Update(DemoCity city);

        bool Delete(int Id);
    }
}
