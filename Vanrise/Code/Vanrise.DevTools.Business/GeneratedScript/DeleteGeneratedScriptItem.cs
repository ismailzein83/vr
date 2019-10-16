﻿using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.DevTools.Entities;
using Vanrise.Common;
using System.Linq;

namespace Vanrise.DevTools.Business
{
    public class DeleteGeneratedScriptItem : GeneratedScriptItemTableSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("411F4A51-1B9A-4221-9F16-1A05C1877EB2"); }

        }
        public override bool Validate(IGeneratedScriptItemTableValidationContext context)
        {
            List<string> Errors = new List<string>();
            if (this.IdentifierColumn == null) Errors.Add("IdentifierColumn cannot be null");
            if (this.KeyValues == null) Errors.Add("KeyValues cannot be null");
            context.Errors = Errors;
            return (Errors.Count != 0) ? false : true;

        }

        public override GeneratedScriptItemTableSettings FindScriptDiffernces(GeneratedScriptItemTableSettings newScriptSettings, GeneratedScriptItemTableSettings oldScriptSettings)
        {
            var newDeleteSettings = newScriptSettings as DeleteGeneratedScriptItem;

            if (newDeleteSettings == null || newDeleteSettings.KeyValues == null || newDeleteSettings.KeyValues.Count == 0)
                return null;

            var oldDeleteSettings = oldScriptSettings as DeleteGeneratedScriptItem;

            if (oldDeleteSettings == null || oldDeleteSettings.KeyValues == null || oldDeleteSettings.KeyValues.Count == 0)
                return newDeleteSettings;

            var keyDifferences = newDeleteSettings.KeyValues.Except(oldDeleteSettings.KeyValues).ToList();

            if (keyDifferences.Count > 0)
                return new DeleteGeneratedScriptItem()
                {
                    IdentifierColumn = newDeleteSettings.IdentifierColumn,
                    KeyValues = newDeleteSettings.KeyValues
                };

            return null;
        }

        public GeneratedScriptItemTableColumn IdentifierColumn { get; set; }
        public List<string> KeyValues { get; set; }
        public override string GenerateQuery(IGeneratedScriptItemTableContext context)
        {
            switch (context.Type)
            {
                case GeneratedScriptType.SQL: return GenerateSQLQuery(context.Item);

                default: return null;

            }


        }
        private string MapRecord(object value)
        {
            if (value is null)
                return "NULL";
            else return string.Format("{0}", value);
        }
        private string GenerateSQLQuery(GeneratedScriptItemTable item)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(@"
                 --- [#Schema#].[#TableName#]-------------------------------------------------------------------
                 -----------------------------------------------------------------------------------------------
                 delete from[#Schema#].[#TableName#] where #IdentifierColumn# IN (#IDs#)
              ");
            queryBuilder.Replace("#Schema#", item.Schema);
            queryBuilder.Replace("#TableName#", item.TableName);
            queryBuilder.Replace("#IdentifierColumn#", this.IdentifierColumn.ColumnName);
            queryBuilder.Replace("#IDs#", string.Join(",", this.KeyValues.MapRecords(x => MapRecord(x))));

            return queryBuilder.ToString();
        }

    }

}
