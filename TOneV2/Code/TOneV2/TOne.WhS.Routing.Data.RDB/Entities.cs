using Vanrise.Entities;

namespace TOne.WhS.Routing.Data.RDB
{
    public class RoutingTableColumnDefinition
    {
        public string ColumnName { get; set; }
        public RDBDataType RDBDataType { get; set; }
        public bool NotNullable { get; set; }
        public int? Size { get; set; }
        public int? Precision { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsIdentity { get; set; }

        public RoutingTableColumnDefinition(string columnName, RDBDataType dataType, bool notNullable = false)
        {
            this.ColumnName = columnName;
            this.RDBDataType = dataType;
            this.NotNullable = notNullable;
        }

        public RoutingTableColumnDefinition(string columnName, RDBDataType dataType, int? size, int? precision, bool notNullable)
        {
            this.ColumnName = columnName;
            this.RDBDataType = dataType;
            this.Size = size;
            this.Precision = precision;
            this.NotNullable = notNullable;
        }

        public RoutingTableColumnDefinition(string columnName, RDBDataType dataType, int? size, int? precision, bool notNullable, bool isPrimaryKey, bool isIdentity)
        {
            this.ColumnName = columnName;
            this.RDBDataType = dataType;
            this.Size = size;
            this.Precision = precision;
            this.NotNullable = notNullable;
            this.IsPrimaryKey = isPrimaryKey;
            this.IsIdentity = isIdentity;
        }
    }
}