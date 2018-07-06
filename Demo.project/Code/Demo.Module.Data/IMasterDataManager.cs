using System;
using Demo.Module.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
    public interface IMasterDataManager : IDataManager
    {
        bool AreMastersUpdated(ref object updateHandle);
        List<Master> GetMasters();
        bool Insert(Master master, out long insertedId);
        bool Update(Master master);
    }
}
