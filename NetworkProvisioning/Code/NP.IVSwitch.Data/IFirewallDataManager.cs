using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NP.IVSwitch.Entities;

namespace NP.IVSwitch.Data
{
    public interface IFirewallDataManager : IDataManager
    {
        List<Firewall> GetFirewalls();
        bool Update(Firewall firewall);
        bool Insert(Firewall firewall, out int insertedId);
    }
}
