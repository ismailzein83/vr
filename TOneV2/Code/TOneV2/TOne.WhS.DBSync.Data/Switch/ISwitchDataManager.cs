using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.DBSync.Data
{
    public interface ISwitchDataManager : IDataManager
    {
        void InsertSwitchFromSource(Switch whsSwitch);
    }
}
