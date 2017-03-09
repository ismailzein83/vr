﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRLoggableEntityDataManager : IDataManager
    {
        Guid AddOrUpdateLoggableEntity( string entityUniqueName, VRLoggableEntitySettings loggableEntitySettings);
        bool AreVRObjectTrackingUpdated(ref object updateHandle);

 
        List<VRLoggableEntity> GetAll();
    }
}
