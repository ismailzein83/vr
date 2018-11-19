using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities.Tools
{
    public class GeneratedScriptItem
    {
        public List<GeneratedScriptItemTable> Tables { get; set; }
    }

    public class GeneratedScriptItemTable
    {
        public Guid ConnectionId { get; set; }
        public string Schema { get; set; }
        public string TableName { get; set; }
        public GeneratedScriptItemTableSettings GeneratedScriptItemTableSettings { get; set; }

    }
    public class GeneratedScriptItemTableColumn
    {
        public string ColumnName { get; set; }
    }

    public class GeneratedScriptItemTableRow
    {
        public Dictionary<string, Object> FieldValues { get; set; }
    }

    public abstract class GeneratedScriptItemTableSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract string ExceptionMessage(GeneratedScriptItemTable generatedScriptItemTable);
    }

    public class DeleteGeneratedScriptItem : GeneratedScriptItemTableSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("411F4A51-1B9A-4221-9F16-1A05C1877EB2"); }

        }

        public override string ExceptionMessage(GeneratedScriptItemTable generatedScriptItemTable)
        {
            if (generatedScriptItemTable.ConnectionId == null) return ("Check ConnectionId");
            if (generatedScriptItemTable.Schema == null) return ("Check Schema");
            if (generatedScriptItemTable.TableName == null) return ("Check Table");
            if (generatedScriptItemTable.GeneratedScriptItemTableSettings == null) return ("Check Settings");
            if (this.IdentifierColumn == null) return ("Check IdentifierColumn");
            if (this.KeyValues == null) return ("Check KeyValues");

            return null;
        }
        public GeneratedScriptItemTableColumn IdentifierColumn { get; set; }
        public List<string> KeyValues { get; set; }
    }

    public class MergeGeneratedScriptItem : GeneratedScriptItemTableSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("9837CAD1-1F8F-4EDF-8AAB-25864C4EF842"); }

        }

        public override string ExceptionMessage(GeneratedScriptItemTable generatedScriptItemTable)
        {
            if (generatedScriptItemTable.ConnectionId == null) return ("Check ConnectionId");
            if (generatedScriptItemTable.Schema == null) return ("Check Schema");
            if (generatedScriptItemTable.TableName == null) return ("Check Table");
            if (generatedScriptItemTable.GeneratedScriptItemTableSettings == null) return ("Check Settings");
            if (this.IdentifierColumns == null) return ("Check IdentifierColumns");
            if (this.UpdateColumns == null) return ("Check UpdateColumns");
            if (this.InsertColumns == null) return ("Check InsertColumns");
            if (this.DataRows == null) return ("Check DataRows");

            return null;
        }
        public List<GeneratedScriptItemTableColumn> InsertColumns { get; set; }
        public List<GeneratedScriptItemTableColumn> UpdateColumns { get; set; }
        public List<GeneratedScriptItemTableColumn> IdentifierColumns { get; set; }
        public List<GeneratedScriptItemTableRow> DataRows { get; set; }

    }

    public class GeneratedScriptItemTableSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Tools_GeneratedScriptSettings";
        public string Editor { get; set; }

    }

}

