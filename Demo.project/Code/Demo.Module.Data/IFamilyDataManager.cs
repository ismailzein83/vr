using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
    public interface IFamilyDataManager:IDataManager
    {
        bool AreCompaniesUpdated(ref object updateHandle);
        List<Family> GetFamilies();
        bool Insert(Family family, out long insertedId);
        bool Update(Family family);
    }
}
