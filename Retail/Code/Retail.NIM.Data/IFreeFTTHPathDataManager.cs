using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Data
{
    public interface IFreeFTTHPathDataManager : IDataManager
    {
        FreeFTTHPath GetFreeFTTHPath(string fdbNumber);
    }
}