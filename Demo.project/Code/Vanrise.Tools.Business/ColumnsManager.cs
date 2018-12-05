using Vanrise.Tools.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Vanrise.Common;
using Vanrise.Tools.Entities;
using Vanrise.Data.SQL;
using Vanrise.Common.Business;
using Vanrise.Entities;
namespace Vanrise.Tools.Business
{
    public class ColumnsManager
    {


        #region Public Methods
        public IEnumerable<ColumnsInfo> GetColumnsInfo(ColumnsInfoFilter columnInfoFilter)
        {
            IColumnsDataManager columnsDataManager = VRToolsFactory.GetDataManager<IColumnsDataManager>();

            Guid connectionId = columnInfoFilter.ConnectionId;
            SQLConnection settings = new VRConnectionManager().GetVRConnection(connectionId).Settings as SQLConnection;
            string connectionString = (settings != null) ? settings.ConnectionString : null;

            columnsDataManager.Connection_String = connectionString;

            List<Columns> allColumns = columnsDataManager.GetColumns(columnInfoFilter.TableName);

            Func<Columns, bool> filterFunc = (columns) =>
            {
                return true;
            };
            return allColumns.MapRecords(ColumnsInfoMapper, filterFunc).OrderBy(columns => columns.Name);
        }

        public IEnumerable<GeneratedScriptItemTableSettingsConfig> GetGeneratedScriptItemTableSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<GeneratedScriptItemTableSettingsConfig>(GeneratedScriptItemTableSettingsConfig.EXTENSION_TYPE);
        }

        
        public List<GeneratedScriptItemValidatorOutput> Validate(GeneratedScriptItemTables generatedScriptItemTables)
        {

            List<GeneratedScriptItemValidatorOutput> results = new List<GeneratedScriptItemValidatorOutput>();
            foreach (var item in generatedScriptItemTables.Scripts)
            {
                
                var generatedScriptItemValidatorOutput = new GeneratedScriptItemValidatorOutput();
                generatedScriptItemValidatorOutput.Errors = new List<string>();
                if (item.Schema == null)
                    generatedScriptItemValidatorOutput.Errors.Add("Schema cannot be null");
                if (item.TableName == null)
                    generatedScriptItemValidatorOutput.Errors.Add("Table cannot be null");
                if (item.Settings == null) generatedScriptItemValidatorOutput.Errors.Add("Settings cannot be null");
                else {
                    var context = new GeneratedScriptValidationContext();
                if(!item.Settings.Validate(context))
                {
                    generatedScriptItemValidatorOutput.Errors.AddRange(context.Errors);
                }
                }
                generatedScriptItemValidatorOutput.TableTitle = item.TableName;
                generatedScriptItemValidatorOutput.IsValidated = generatedScriptItemValidatorOutput.Errors.Count == 0 ? true : false;
                results.Add(generatedScriptItemValidatorOutput);
            }

            return results;
        }

        public string GenerateQueries(GeneratedScriptItem generatedScriptItem)
        {

        StringBuilder queriesBuilder = new StringBuilder();
            foreach (var item in generatedScriptItem.Tables.Scripts) {
                queriesBuilder.AppendLine();
                GeneratedScriptItemTableContext context = new GeneratedScriptItemTableContext();
                context.Item = item;
                context.Type = generatedScriptItem.Type;
                queriesBuilder.Append(item.Settings.GenerateQuery(context));
            }
            return queriesBuilder.ToString();
        }

        public string GetGeneratedQueries(string jsonScripts, GeneratedScriptType type)
        {
            var generatedScriptItem = new GeneratedScriptItem
            {
                Tables = new GeneratedScriptItemTables(),
                Type = type
            };
            generatedScriptItem.Tables.Scripts = Vanrise.Common.Serializer.Deserialize<List<GeneratedScriptItemTable>>(jsonScripts);
            return GenerateQueries(generatedScriptItem);
        }

        public string GenerateQueriesFromTextFile(string filePath, GeneratedScriptType type)
        {
            return GetGeneratedQueries(System.IO.File.ReadAllText(filePath), type);
        }

        #endregion

        #region Private Classes

        #endregion

        #region Private Methods

        #endregion

        #region Mappers

        public ColumnsInfo ColumnsInfoMapper(Columns columns)
        {
            return new ColumnsInfo
            {
                Name = columns.Name,
            };
        }
        #endregion

    }
}
