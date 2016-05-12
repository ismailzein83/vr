﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public interface ISecurityContext
    {
        int GetLoggedInUserId();

        bool TryGetLoggedInUserId(out int? userId);

        bool IsAllowed(string requiredPermissions);

        bool HasPermissionToActions(string systemActionNames);
    }
}
