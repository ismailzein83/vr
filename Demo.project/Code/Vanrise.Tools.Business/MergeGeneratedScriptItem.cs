using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
namespace Vanrise.Tools.Business
{
    public class MergeGeneratedScriptItem : GeneratedScriptItemTableSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("9837CAD1-1F8F-4EDF-8AAB-25864C4EF842"); }

        }
        public override bool Validate(IGeneratedScriptItemTableValidationContext context)
        {
            var errors = new List<string>();

            if (this.IdentifierColumns == null)
                errors.Add("IdentifierColumns cannot be null");
            if (this.UpdateColumns == null)
                errors.Add("UpdateColumns cannot be null");
            if (this.InsertColumns == null)
                errors.Add("InsertColumns cannot be null");
            if (this.DataRows == null)
                errors.Add("DataRows cannot be null");

            if(errors.Count > 0)
            {
                context.Errors = errors;
                return false;
            }
            return true;
        }
        public override string GenerateQuery(IGeneratedScriptItemTableContext context)
        {
            switch (context.Type)
            {
                case GeneratedScriptType.SQL: return GenerateSQLQuery(context.Item);

                default: return null;

            }

        }
        public List<GeneratedScriptItemTableColumn> InsertColumns { get; set; }
        public List<GeneratedScriptItemTableColumn> UpdateColumns { get; set; }
        public List<GeneratedScriptItemTableColumn> IdentifierColumns { get; set; }
        public List<GeneratedScriptItemTableRow> DataRows { get; set; }
        public GeneratedScriptQueryType? QueryType { get; set; }
        public bool IsIdentity { get; set; }
        private string GenerateSQLQuery(GeneratedScriptItemTable item)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(@"
                 --- [#Schema#].[#TableName#]-------------------------------------------------------------------
                 -----------------------------------------------------------------------------------------------
                 begin
                 set nocount on;
                 #IdentityOn#
                 ;with cte_data(#Columns#)
                  as (select* from (values
                 --//////////////////////////////////////////////////////////////////////////////////////////////////
                       #Values#
                 --\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                )c(#Columns#))
                merge[#Schema#].[#TableName#] as t
                using  cte_data as s
                on            1=1 and #IdentifierColumns#
                  #QueryType#
                  #IdentityOff#
            ----------------------------------------------------------------------------------------------------
              end
            ----------------------------------------------------------------------------------------------------
              ");

            StringBuilder valuesBuilder = new StringBuilder();
            foreach (var row in this.DataRows)
            {
                if (valuesBuilder.Length > 0)
                {
                    valuesBuilder.Append(",");
                    valuesBuilder.AppendLine();
                }
                valuesBuilder.Append("(");
                valuesBuilder.Append(string.Join(",", row.FieldValues.MapRecords(x => string.Format("'{0}'", x.Value))));
                valuesBuilder.Append(")");
            }



            StringBuilder identityOnBuilder = new StringBuilder();
            StringBuilder identityOffBuilder = new StringBuilder();

            if (this.IsIdentity)
            {
                identityOnBuilder.Append(@"set identity_insert [#Schema#].[#TableName#] on;");
                identityOnBuilder.Replace("#Schema#", item.Schema).Replace("#TableName#", item.TableName);
                identityOffBuilder.Append(@"set identity_insert [#Schema#].[#TableName#] off;");
                identityOffBuilder.Replace("#Schema#", item.Schema).Replace("#TableName#", item.TableName);
            }

            StringBuilder queryTypeBuilder = new StringBuilder();
            if (!this.QueryType.HasValue || this.QueryType.Value == GeneratedScriptQueryType.Update)
            {
                queryTypeBuilder.Append(@"
                  when matched then
                 update set
                 #Update#");

                queryTypeBuilder.Replace("#Update#", string.Join(",", this.UpdateColumns.MapRecords(x => string.Format("[{0}]=s.[{0}] ", x.ColumnName))));
            }
            if (!this.QueryType.HasValue || this.QueryType.Value == GeneratedScriptQueryType.Insert)
            {
                queryTypeBuilder.Append(@"
                 when not matched by target then
                 insert(#InsertColumns#)
                 values(#InsertColumnsValues#)");

                queryTypeBuilder.Replace("#InsertColumns#", string.Join(",", this.InsertColumns.MapRecords(x => string.Format("[{0}]", x.ColumnName))));
                queryTypeBuilder.Replace("#InsertColumnsValues#", string.Join(", ", this.InsertColumns.MapRecords(x => string.Format("s.[{0}]", x.ColumnName))));
            }
            queryTypeBuilder.Append(";");

            queryBuilder.Replace("#Columns#", string.Join(",", this.DataRows[0].FieldValues.MapRecords(x => string.Format("[{0}]", x.Key))));
            queryBuilder.Replace("#Values#", valuesBuilder.ToString());
            queryBuilder.Replace("#Schema#", item.Schema);
            queryBuilder.Replace("#TableName#", item.TableName);
            queryBuilder.Replace("#IdentifierColumns#", string.Join(" and ", this.IdentifierColumns.MapRecords(x => string.Format("t.[{0}]=s.[{0}]", x.ColumnName))));
            queryBuilder.Replace("#QueryType#", queryTypeBuilder.ToString());
            queryBuilder.Replace("#IdentityOn#", identityOnBuilder.ToString());
            queryBuilder.Replace("#IdentityOff#", identityOffBuilder.ToString());

            return queryBuilder.ToString();
        }

    }
    public enum GeneratedScriptQueryType { Insert = 1, Update = 2 }
}
