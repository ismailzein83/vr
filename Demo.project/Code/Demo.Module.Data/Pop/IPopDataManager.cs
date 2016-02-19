using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;


namespace Demo.Module.Data
{
    public interface IPopDataManager : IDataManager
    {
        List<Pop> GetAllPops();
        bool Insert(Pop pop,out int popId);
        bool Update(Pop pop);
        bool ArePopsUpdated(ref object updateHandle);
    }
}
