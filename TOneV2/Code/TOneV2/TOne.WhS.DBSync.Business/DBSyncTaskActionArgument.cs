namespace TOne.WhS.DBSync.Business
{
    public class DBSyncTaskActionArgument : Vanrise.Runtime.Entities.BaseTaskActionArgument
    {
        public string ConnectionString { get; set; }
        public bool UseTempTables { get; set; }
        public int DefaultSellingNumberPlanId { get; set; }
    }
}
