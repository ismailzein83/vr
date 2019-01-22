using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.DevTools.Entities;
using Vanrise.Common;

namespace Vanrise.DevTools.MainExtensions
{
    public class SelectGeneratedScriptVariable : GeneratedScriptVariableSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("1F5BDDAD-B1C2-4056-BEC6-44233DE11CF2"); }

        }

        public string TableName { get; set; }
        public string SchemaName { get; set; }
        public string ColumnName { get; set; }
        public string FilterColumnName { get; set; }
        public string FilterValue { get; set; }
        public override string GetSQLVariableSettings(IGeneratedScriptVariableContext context)
        {
            StringBuilder variableBuilder = new StringBuilder();

            variableBuilder.Append(string.Format("SELECT {0} FROM {1}.[{2}] where [{3}]={4}", ColumnName, SchemaName, TableName, FilterColumnName, FilterValue));

            return variableBuilder.ToString();

    }
}
  
 
}