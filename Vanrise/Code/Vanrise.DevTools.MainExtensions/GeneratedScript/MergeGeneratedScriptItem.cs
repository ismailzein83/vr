﻿using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.DevTools.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Data.RDB;
namespace Vanrise.DevTools.MainExtensions
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

            if (this.Columns == null)
                errors.Add("Columns cannot be null");
            if (this.DataRows == null)
                errors.Add("DataRows cannot be null");

            if (errors.Count > 0)
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
        public List<MergeGeneratedScriptItemColumn> Columns { get; set; }
        public List<GeneratedScriptItemTableRow> DataRows { get; set; }
        public List<GeneratedScriptVariable> Variables { get; set; }
       

        public string LastWhereCondition { get; set; }
        public string LastJoinStatement { get; set; }

        public bool IsIdentity { get; set; }
        private string MapRecord(object value)
        {
                GeneratedScriptVariableData variable = value as GeneratedScriptVariableData;
            if(variable !=null)
                return "@" + Variables.Find(x => x.Id == variable.VariableId).Name + '_'+variable.VariableId.ToString("N");
            if (value is null)
                return "NULL";
            if (value is int || value is float || value is decimal || value is long || value is double)
                return string.Format("{0}", value);
            if (value is DateTime)
                return $"'{((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
            else return string.Format("'{0}'", value.ToString().Replace("'", "''"));
        }
        private string GenerateSQLQuery(GeneratedScriptItemTable item)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(@"
                 --- [#Schema#].[#TableName#]-------------------------------------------------------------------
                 -----------------------------------------------------------------------------------------------
                 begin
                 #Variables#
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

            StringBuilder variablesBuilder = new StringBuilder();
            
            foreach (var variable in this.Variables)
            {
                var context = new GeneratedScriptVariableContext()
                {
                    Name = variable.Name,
                    Description = RDBDataTypeAttribute.GetAttribute(variable.Type).Description,
                    Size = variable.Size,
                    Precision = variable.Precision
                };

                variablesBuilder.Append(string.Format("Declare @{0}_{1} ",variable.Name.Replace(" ", String.Empty),variable.Id.ToString("N")));
                variablesBuilder.Append(GetType(variable.Type,variable.Size,variable.Precision).ToString());
                variablesBuilder.Append(variable.Settings.GetSQLVariableSettings(context));
                variablesBuilder.AppendLine();
            }

            StringBuilder valuesBuilder = new StringBuilder();
            foreach (var row in this.DataRows)
            {
                if (valuesBuilder.Length > 0)
                {
                    
                    valuesBuilder.Append(",");
                    valuesBuilder.AppendLine();
                }
                valuesBuilder.Append("(");
                valuesBuilder.Append(string.Join(",", row.FieldValues.MapRecords(x => MapRecord(x.Value))));
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
            if (Columns.Exists(x=>x.IncludeInUpdate))
            {
                queryTypeBuilder.Append(@"
                  when matched then
                 update set
                 #Update#");

                queryTypeBuilder.Replace("#Update#", string.Join(",", this.Columns.MapRecords(x => string.Format("[{0}]=s.[{0}] ", x.ColumnName), x =>x.IncludeInUpdate)));
            }
            if (Columns.Exists(x => x.IncludeInInsert))
            {
                queryTypeBuilder.Append(@"
                 when not matched by target then
                 insert(#InsertColumns#)
                 values(#InsertColumnsValues#)");

                queryTypeBuilder.Replace("#InsertColumns#", string.Join(",", this.Columns.MapRecords(x => string.Format("[{0}]", x.ColumnName),x=>x.IncludeInInsert)));
                queryTypeBuilder.Replace("#InsertColumnsValues#", string.Join(", ", this.Columns.MapRecords(x => string.Format("s.[{0}]", x.ColumnName),x=>x.IncludeInInsert)));
            }
            queryTypeBuilder.Append(";");
            queryBuilder.Replace("#Variables#", variablesBuilder.ToString());
            queryBuilder.Replace("#Columns#", string.Join(",", this.Columns.MapRecords(x => string.Format("[{0}]", x.ColumnName))));
            queryBuilder.Replace("#Values#", valuesBuilder.ToString());
            queryBuilder.Replace("#Schema#", item.Schema);
            queryBuilder.Replace("#TableName#", item.TableName);
            queryBuilder.Replace("#IdentifierColumns#", string.Join(" and ", this.Columns.MapRecords(x => string.Format("t.[{0}]=s.[{0}]", x.ColumnName),x=>x.IsIdentifier)));
            queryBuilder.Replace("#QueryType#", queryTypeBuilder.ToString());
            queryBuilder.Replace("#IdentityOn#", identityOnBuilder.ToString());
            queryBuilder.Replace("#IdentityOff#", identityOffBuilder.ToString());

            return queryBuilder.ToString();
        }
        private string GetType(RDBDataType rdbDataType, int? size, int? precision)
        {
            
             switch (rdbDataType)
            {
                case RDBDataType.Decimal:
                    return string.Format("Decimal({0},{1}) ", size,precision) ;
                case RDBDataType.Varchar:
                case RDBDataType.NVarchar:
                case RDBDataType.VarBinary:
                    return string.Format("{0}({1}) ", RDBDataTypeAttribute.GetAttribute(rdbDataType).Description,size.HasValue?size.Value.ToString():"MAX");
                case RDBDataType.Int:
                case RDBDataType.Boolean:
                case RDBDataType.Cursor:
                case RDBDataType.DateTime:
                case RDBDataType.UniqueIdentifier:
                case RDBDataType.BigInt:
                    return string.Format("{0} ", RDBDataTypeAttribute.GetAttribute(rdbDataType).Description);
                default: return null;
            }

        }

    }

    public class MergeGeneratedScriptItemColumn
    {
        public string ColumnName { get; set; }

        public bool IsIdentifier { get; set; }

        public bool IncludeInInsert { get; set; }

        public bool IncludeInUpdate { get; set; }
    }
}
