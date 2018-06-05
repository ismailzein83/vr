using Demo.BestPractices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BestPractices.Data
{
    public interface IParentDataManager:IDataManager
    {
        bool AreParentsUpdated(ref object updateHandle);
        List<Parent> GetParents();
        bool Insert(Parent parent, out long insertedId);
        bool Update(Parent parent);
    }
}
