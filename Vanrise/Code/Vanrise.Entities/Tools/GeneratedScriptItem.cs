using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class GeneratedScriptItem
    {
        public GeneratedScriptType Type { get; set; }
        public GeneratedScriptItemTables Tables { get; set; }
    }

    public class GeneratedScriptItemTables
    {
        public  List<GeneratedScriptItemTable> Scripts { get; set; }

    }

    public enum GeneratedScriptType { SQL = 1 }

    public class GeneratedScriptItemTable
    {
        public Guid ConnectionId { get; set; }
        public string Schema { get; set; }
        public string TableName { get; set; }
        public GeneratedScriptItemTableSettings Settings { get; set; }

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
        public abstract bool Validate(IGeneratedScriptItemTableValidationContext context);
        public abstract string GenerateQuery(IGeneratedScriptItemTableContext context);

    }

    public interface IGeneratedScriptItemTableContext
    {
         GeneratedScriptItemTable Item { get; }
         GeneratedScriptType Type {  get; }
    }
    public class GeneratedScriptItemTableContext : IGeneratedScriptItemTableContext
    {
        public GeneratedScriptItemTable Item { get; set; }
        public GeneratedScriptType Type { get; set; }
    }
    public interface IGeneratedScriptItemTableValidationContext
    {
         List<string> Errors { set; }
    }
    public class GeneratedScriptValidationContext: IGeneratedScriptItemTableValidationContext
    {
        public List<string> Errors { get; set; }
    }
    public class GeneratedScriptItemValidatorOutput
    {
        public string TableTitle { get; set; }
        public List<string> Errors { get; set; }
        public bool IsValidated { get; set; }
    }



    public class GeneratedScriptItemTableSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Tools_GeneratedScriptSettings";
        public string Editor { get; set; }

    }

}

