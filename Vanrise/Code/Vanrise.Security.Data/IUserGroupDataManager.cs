﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface IUserGroupDataManager : IDataManager
    {
        IEnumerable<UserGroup> GetAllUserGroupEntities();

        bool AreUserGroupUpdated(ref object updateHandle);
    }
}
