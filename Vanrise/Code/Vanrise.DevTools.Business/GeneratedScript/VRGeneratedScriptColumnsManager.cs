using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.DevTools.Data;
using Vanrise.DevTools.Entities;

namespace Vanrise.DevTools.Business
{
    public class VRGeneratedScriptColumnsManager
    {
        #region Public Methods
        public IEnumerable<VRGeneratedScriptColumnsInfo> GetColumnsInfo(VRGeneratedScriptColumnsInfoFilter columnInfoFilter)
        {
            IVRGeneratedScriptColumnsDataManager columnsDataManager = VRDevToolsFactory.GetDataManager<IVRGeneratedScriptColumnsDataManager>();

            Guid connectionId = columnInfoFilter.ConnectionId;
            SQLConnection settings = new VRConnectionManager().GetVRConnection(connectionId).Settings as SQLConnection;

            if (settings != null)
            {
                if (settings.ConnectionString != null)
                    columnsDataManager.Connection_String = settings.ConnectionString;
                else if (settings.ConnectionStringAppSettingName != null)
                    columnsDataManager.Connection_String = settings.ConnectionStringAppSettingName;
                else
                    columnsDataManager.Connection_String = settings.ConnectionStringName;
            }
            List<VRGeneratedScriptColumns> allColumns = columnsDataManager.GetColumns(columnInfoFilter.TableName);

            Func<VRGeneratedScriptColumns, bool> filterFunc = (column) =>
            {
                if (columnInfoFilter.ColumnNames != null && columnInfoFilter.ColumnNames.Count > 0)
                {
                    return columnInfoFilter.ColumnNames.Any(x => x.Name != "timestamp" && x.Name == column.Name);
                }
                if (column.Name == "timestamp")
                    return false;
                return true;
            };
            return allColumns.MapRecords(ColumnsInfoMapper, filterFunc);
        }

        public IEnumerable<GeneratedScriptItemTableSettingsConfig> GetGeneratedScriptItemTableSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<GeneratedScriptItemTableSettingsConfig>(GeneratedScriptItemTableSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<GeneratedScriptVariableSettingsConfig> GetGeneratedScriptVariableSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<GeneratedScriptVariableSettingsConfig>(GeneratedScriptVariableSettingsConfig.EXTENSION_TYPE);
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
                else
                {
                    var context = new GeneratedScriptValidationContext();
                    if (!item.Settings.Validate(context))
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
        public string CompareJsonScripts(string newScripts, string oldScripts)
        {
            GeneratedScriptItemTables newItems = Serializer.Deserialize<GeneratedScriptItemTables>(newScripts);
            GeneratedScriptItemTables oldItems = Serializer.Deserialize<GeneratedScriptItemTables>(oldScripts);

            if (newItems == null || newItems.Scripts == null || newItems.Scripts.Count == 0)
                return null;

            if (oldItems == null || oldItems.Scripts == null || oldItems.Scripts.Count == 0)
                return Serializer.Serialize(newItems);

            GeneratedScriptItemTables differentItems = new GeneratedScriptItemTables() { Scripts = new List<GeneratedScriptItemTable>() }; ;

            foreach (var newScript in newItems.Scripts)
            {
                bool isScriptFound = false;

                foreach (var oldScript in oldItems.Scripts)
                {
                    if (newScript.ConnectionId == oldScript.ConnectionId && newScript.Schema == oldScript.Schema && newScript.TableName == oldScript.TableName && newScript.Settings.ConfigId.ToString() == oldScript.Settings.ConfigId.ToString())
                    {
                        isScriptFound = true;

                        if (oldScript.Settings != null)
                        {
                            var settingsDifferences = oldScript.Settings.FindScriptDiffernces(newScript.Settings, oldScript.Settings);
                            if (settingsDifferences != null)
                            {
                                differentItems.Scripts.Add(new GeneratedScriptItemTable()
                                {
                                    ConnectionId = newScript.ConnectionId,
                                    Schema = newScript.Schema,
                                    TableName = newScript.TableName,
                                    Settings = settingsDifferences
                                });
                            }
                        }

                        break;
                    }
                }

                if (!isScriptFound && newScript.Settings != null)
                    differentItems.Scripts.Add(newScript);
            }
            if (differentItems.Scripts.Count == 0)
                return null;

            return Serializer.Serialize(differentItems);
        }
        public List<GeneratedScriptItemComparisonOutput> CompareItems(GeneratedScriptItem generatedScriptItem)
        {
            List<GeneratedScriptItemComparisonOutput> comparisonOutputs = new List<GeneratedScriptItemComparisonOutput>();
            if (generatedScriptItem != null && generatedScriptItem.Tables != null && generatedScriptItem.Tables.Scripts != null && generatedScriptItem.Tables.Scripts.Count > 0)
            {
                foreach (var script in generatedScriptItem.Tables.Scripts)
                {
                    comparisonOutputs.Add(script.Settings.Compare(new GeneratedScriptComparisonContext { ConnectionId = script.ConnectionId, Schema = script.Schema, TableName = script.TableName, IncludeOverriddenValuesInComparison = generatedScriptItem.IncludeOverriddenValuesInComparison }));
                }
            }
            return comparisonOutputs;
        }

        public string GenerateQueries(GeneratedScriptItem generatedScriptItem)
        {

            StringBuilder queriesBuilder = new StringBuilder();
            foreach (var item in generatedScriptItem.Tables.Scripts)
            {
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
        public string MapRecord(object value)
        {
            if (value.GetType() == null || value.GetType() == typeof(int) || value.GetType() == typeof(float) || value.GetType() == typeof(decimal) || value.GetType() == typeof(long) || value.GetType() == typeof(double))
                return string.Format("{0}", value);
            else return string.Format("'{0}'", value);
        }
        #endregion

        #region Private Classes

        #endregion

        #region Private Methods

        #endregion

        #region Mappers
        public VRGeneratedScriptColumnsInfo ColumnsInfoMapper(VRGeneratedScriptColumns columns)
        {
            return new VRGeneratedScriptColumnsInfo
            {
                Name = columns.Name,
            };
        }
        #endregion

    }
}
