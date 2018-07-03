﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Data
{
    public interface IVRSequenceDataManager : IDataManager
    {
        long GetNextSequenceValue(string sequenceGroup, Guid sequenceDefinitionId, string sequenceKey, long initialValue, long? reserveNumber);
    }
}
