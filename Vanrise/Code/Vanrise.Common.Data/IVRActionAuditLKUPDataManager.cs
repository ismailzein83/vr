using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRActionAuditLKUPDataManager : IDataManager
    {
        int AddLKUPIfNotExists(VRActionAuditLKUPType lkupType, string name);

        List<VRActionAuditLKUP> GetAll();
    }
}
