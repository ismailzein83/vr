using Demo.Module.Entities;
using System.Collections.Generic;

namespace Demo.Module.Data
{
    public interface IManufactoryDataManager : IDataManager
    {
        List<Manufactory> GetManufactories();

        bool InsertManufactory(Manufactory manufactory, out int insertedId);

        bool UpdateManufactory(Manufactory manufactory);

        bool AreManufactoriesUpdated(ref object updateHandle);
    }
}