using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IOverriddenConfigurationGroupDataManager : IDataManager
    {
        bool Insert(OverriddenConfigurationGroup overriddenConfigurationGroup);
        List<OverriddenConfigurationGroup> GetOverriddenConfigurationGroup();
        bool AreOverriddenConfigurationGroupUpdated(ref object updateHandle);

        void GenerateScript(List<OverriddenConfigurationGroup> groups, Action<string, string> addEntityScript);
    }
}
