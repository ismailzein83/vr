
namespace TOne.WhS.DBSync.Entities
{
    public class DBForeignKey
    {
        public string ReferencedKey { get; set; }
        public string ReferencedTable { get; set; }
        public string ReferencedTableSchema { get; set; }
    }
}
