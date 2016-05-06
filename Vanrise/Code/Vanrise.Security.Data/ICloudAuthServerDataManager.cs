using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface ICloudAuthServerDataManager : IDataManager
    {
        CloudAuthServer GetAuthServer();
        
        bool IsAuthServerUpdated(ref object updateHandle);

        bool InsertCloudAuthServer(CloudAuthServer cloudAuthServer);

        bool UpdateCloudAuthServer(CloudAuthServer cloudAuthServer);
    }
}
