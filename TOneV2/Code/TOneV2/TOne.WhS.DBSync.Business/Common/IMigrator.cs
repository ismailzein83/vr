
using TOne.WhS.DBSync.Entities;
namespace TOne.WhS.DBSync.Business
{
    public interface IMigrator
    {
        void Migrate(MigrationInfoContext context);

        void FillTableInfo(bool useTempTables);
    }
}
