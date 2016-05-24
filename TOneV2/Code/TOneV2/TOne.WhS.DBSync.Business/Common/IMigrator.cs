
namespace TOne.WhS.DBSync.Business
{
    public interface IMigrator
    {
        void Migrate();

        void FillTableInfo(bool useTempTables);
    }
}
