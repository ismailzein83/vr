using Demo.BestPractices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BestPractices.Data
{
    public interface IChildDataManager : IDataManager
    {
        List<Child> GetChilds();
        bool Insert(Child child, out long insertedId);
        bool Update(Child child);
        bool AreChildsUpdated(ref object updateHandle);
    }
}