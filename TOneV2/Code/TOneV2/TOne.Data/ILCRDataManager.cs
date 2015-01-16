using System;
using System.Collections.Generic;
using TOne.Entities;
namespace TOne.Data
{
    public interface ILCRDataManager
    {
        void LoadUpdatedCodes(byte[] timestamp, Action<List<LCRCode>> onCodeGroupListReady);

        List<string> GetUpdatedCodeGroups(byte[] p);
    }
}
