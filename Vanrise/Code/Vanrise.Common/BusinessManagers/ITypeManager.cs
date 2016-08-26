using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common
{
    public interface ITypeManager : IBEManager
    {
        int GetTypeId(Type t);

        int GetTypeId(IVanriseType vanriseType);

        int GetTypeId(string typeUniqueName);
    }
}
