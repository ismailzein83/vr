using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Aspose.Cells;
using System.Drawing;
using System.IO;
using Vanrise.GenericData.Entities.GenericRules;

namespace Vanrise.GenericData.Business
{
    public class GenericRuleDefinitionManager : IGenericRuleDefinitionManager
    {
        #region Public Methods

        public IGenericRuleManager GetManager(Guid ruleDefinitionId)
        {
            GenericRuleDefinition ruleDefinition = GetGenericRuleDefinition(ruleDefinitionId);
            ruleDefinition.ThrowIfNull("ruleDefinition", ruleDefinitionId);
            GenericRuleTypeConfigManager ruleTypeManager = new GenericRuleTypeConfigManager();
            GenericRuleTypeConfig ruleTypeConfig = ruleTypeManager.GetGenericRuleTypeById(ruleDefinition.SettingsDefinition.ConfigId);
            ruleTypeConfig.ThrowIfNull("ruleTypeConfig", ruleDefinition.SettingsDefinition.ConfigId);
            Type managerType = Type.GetType(ruleTypeConfig.RuleManagerFQTN);
            var instance = Activator.CreateInstance(managerType);
            return instance.CastWithValidate<IGenericRuleManager>("instance");
        }
        public Vanrise.Entities.IDataRetrievalResult<GenericRuleDefinition> GetFilteredGenericRuleDefinitions(Vanrise.Entities.DataRetrievalInput<GenericRuleDefinitionQuery> input)
        {
            var cachedGenericRuleDefinitions = GetCachedGenericRuleDefinitions();
            Func<GenericRuleDefinition, bool> filterExpression = (genericRuleDefinition) => (input.Query.Name == null || genericRuleDefinition.Name.ToUpper().Contains(input.Query.Name.ToUpper()));
            VRActionLogger.Current.LogGetFilteredAction(GenericRuleDefinitionLoggableEntity.Instance, input);
            return DataRetrievalManager.Instance.ProcessResult(input, cachedGenericRuleDefinitions.ToBigResult(input, filterExpression));
        }

        public GenericRuleDefinition GetGenericRuleDefinition(Guid genericRuleDefinitionId)
        {
            var cachedGenericRuleDefinitions = GetCachedGenericRuleDefinitions();

            return cachedGenericRuleDefinitions.GetRecord(genericRuleDefinitionId);
        }
       
        public IEnumerable<GenericRuleDefinition> GetGenericRulesDefinitons()
        {
            return this.GetCachedGenericRuleDefinitions().Values;
        }

        public Vanrise.Entities.InsertOperationOutput<GenericRuleDefinition> AddGenericRuleDefinition(GenericRuleDefinition genericRuleDefinition)
        {
            InsertOperationOutput<GenericRuleDefinition> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<GenericRuleDefinition>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
             genericRuleDefinition.GenericRuleDefinitionId = Guid.NewGuid();

            IGenericRuleDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericRuleDefinitionDataManager>();
            bool added = dataManager.AddGenericRuleDefinition(genericRuleDefinition);

            if (added)
            {
                VRActionLogger.Current.TrackAndLogObjectAdded(GenericRuleDefinitionLoggableEntity.Instance, genericRuleDefinition);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = genericRuleDefinition;

                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<GenericRuleDefinition> UpdateGenericRuleDefinition(GenericRuleDefinition genericRuleDefinition)
        {
            UpdateOperationOutput<GenericRuleDefinition> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<GenericRuleDefinition>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = genericRuleDefinition;

            IGenericRuleDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericRuleDefinitionDataManager>();
            bool added = dataManager.UpdateGenericRuleDefinition(genericRuleDefinition);

            if (added)
            {
                VRActionLogger.Current.TrackAndLogObjectUpdated(GenericRuleDefinitionLoggableEntity.Instance, genericRuleDefinition);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;

                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<GenericRuleDefinitionInfo> GetGenericRuleDefinitionsInfo(GenericRuleDefinitionInfoFilter filter)
        {
            var cachedGenericRuleDefinitions = GetCachedGenericRuleDefinitions();
            Func<GenericRuleDefinition, bool> filterExpression = null;

            if (filter != null)
            {
                filterExpression = (item) =>   (item.SettingsDefinition != null && item.SettingsDefinition.ConfigId == filter.RuleTypeId)  || (filter.Filters != null && CheckIfFilterIsMatch(item, filter.Filters));
            }

            return cachedGenericRuleDefinitions.MapRecords(GenericRuleDefinitionInfoMapper, filterExpression);
        }
        public bool CheckIfFilterIsMatch(GenericRuleDefinition ruleDefinition,List<IGenericRuleDefinitionFilter> filters)
        {                    
            GenericRuleDefinitionFilterContext context = new GenericRuleDefinitionFilterContext{ RuleDefinition = ruleDefinition};
            foreach(var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }
        public IEnumerable<GenericRuleDefinition> GetGenericRuleDefinitionsByType(string ruleTypeName)
        {
            GenericRuleTypeConfigManager configManager = new GenericRuleTypeConfigManager();
            var ruleTypeConfigId = configManager.GetGenericRuleTypeIdByName(ruleTypeName);
            Func<GenericRuleDefinition, bool> filterExpression = (item) => (item.SettingsDefinition != null && item.SettingsDefinition.ConfigId == ruleTypeConfigId);
            return GetCachedGenericRuleDefinitions().FindAllRecords(filterExpression);
        }

        public string GetGenericRuleDefinitionName(Guid genericRuleDefinitionId)
        {
            var genericRuleDefinition = GetGenericRuleDefinition(genericRuleDefinitionId);
            return genericRuleDefinition != null ? genericRuleDefinition.Title : null;
        }

        public bool DoesUserHaveViewAccess(Guid genericRuleDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            var genericRuleDefinition = GetGenericRuleDefinition(genericRuleDefinitionId);
            return DoesUserHaveViewAccess(userId, genericRuleDefinition);
        }

        public bool DoesRuleSupportUpload(Guid genericRuleDefinitionId)
        {
            var genericRuleDefinition = GetGenericRuleDefinition(genericRuleDefinitionId);
            genericRuleDefinition.ThrowIfNull("genericRuleDefinition", genericRuleDefinitionId);
            genericRuleDefinition.SettingsDefinition.ThrowIfNull("genericRuleDefinition.SettingsDefinition", genericRuleDefinitionId);
            return genericRuleDefinition.SettingsDefinition.SupportUpload;
        }

        public bool DoesUserHaveViewAccess(GenericRuleDefinition genericRuleDefinition)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveViewAccess(userId, genericRuleDefinition);
        }
        public bool DoesUserHaveViewAccess(int userId, List<Guid> RuleDefinitionIds)
        {
            foreach (var guid in RuleDefinitionIds)
            {
                var genericRuleDefinition = GetGenericRuleDefinition(guid);
                if (DoesUserHaveViewAccess(userId, genericRuleDefinition))
                    return true;
            }
            return false;
        }

        public bool DoesUserHaveViewAccess(int userId, GenericRuleDefinition genericRuleDefinition)
        {
            if (genericRuleDefinition.Security != null && genericRuleDefinition.Security.ViewRequiredPermission != null)
                return DoesUserHaveAccess(userId, genericRuleDefinition.Security.ViewRequiredPermission);
            else
                return true;
        }

        public bool DoesUserHaveAddAccess(Guid genericRuleDefinitionId)
        {
            var genericRuleDefinition = new GenericRuleDefinitionManager().GetGenericRuleDefinition(genericRuleDefinitionId);
            if (genericRuleDefinition != null && genericRuleDefinition.Security != null && genericRuleDefinition.Security.AddRequiredPermission != null)
                return DoesUserHaveAccess(genericRuleDefinition.Security.AddRequiredPermission);
            return true;
        }
        public bool DoesUserHaveEditAccess(Guid genericRuleDefinitionId)
        {
            var genericRuleDefinition = new GenericRuleDefinitionManager().GetGenericRuleDefinition(genericRuleDefinitionId);
            if (genericRuleDefinition != null && genericRuleDefinition.Security != null && genericRuleDefinition.Security.EditRequiredPermission != null)
                return DoesUserHaveAccess(genericRuleDefinition.Security.EditRequiredPermission);
            return true;
        }

        public byte[] DownloadGenericRulesTemplate(Guid genericRuleDefinitionId, List<string> criteriaFieldsToHide)
        {
            int rowIndex = 0;
            int colIndex = 0;

            Workbook GenericRuleTemplate = new Workbook();
            Vanrise.Common.Utilities.ActivateAspose();
            GenericRuleTemplate.Worksheets.Clear();
            Worksheet GenericRuleWorksheet = GenericRuleTemplate.Worksheets.Add("Template");
            var genericRuleDefinition = new GenericRuleDefinitionManager().GetGenericRuleDefinition(genericRuleDefinitionId);
            genericRuleDefinition.ThrowIfNull("genericRuleDefinition", genericRuleDefinitionId);
            genericRuleDefinition.CriteriaDefinition.ThrowIfNull("genericRuleDefinition.CriteriaDefinition", genericRuleDefinitionId);
            var criteriaFields = genericRuleDefinition.CriteriaDefinition.Fields;
            criteriaFields.ThrowIfNull("genericRuleDefinition.CriteriaDefinition.Fields", genericRuleDefinitionId);

            List<string> headers = new List<string>();
            headers.Add("Description");

            foreach(var criteria in criteriaFields)
            {
                if (criteriaFieldsToHide != null)
                {
                    if (criteriaFieldsToHide.Contains(criteria.FieldName))
                        continue;
                }
                headers.Add(criteria.Title);
            }
            genericRuleDefinition.SettingsDefinition.ThrowIfNull("genericRuleDefinition.SettingsDefinition", genericRuleDefinitionId);
            var settingFields = genericRuleDefinition.SettingsDefinition.GetFieldNames();
            settingFields.ThrowIfNull("genericRuleDefinition.SettingsDefinition", genericRuleDefinitionId);
            
            foreach (var settingField in settingFields)
                headers.Add(settingField);

            foreach (var header in headers)
            {
                GenericRuleWorksheet.Cells.SetColumnWidth(colIndex, 20);
                GenericRuleWorksheet.Cells[rowIndex, colIndex].PutValue(header);
                Cell cell = GenericRuleWorksheet.Cells.GetCell(rowIndex, colIndex);
                Style style = cell.GetStyle();
                style.Font.Name = "Times New Roman";
                style.Font.Color = Color.Black;
                style.Font.Size = 14;
                style.Font.IsBold = true;
                cell.SetStyle(style);
                colIndex++;
            }

            MemoryStream memoryStream = new MemoryStream();
            memoryStream = GenericRuleTemplate.SaveToStream();

            VRFile file = new VRFile()
            {
                Content = memoryStream.ToArray()
            };
            return file.Content;
        }

        public UploadGenericRulesOutput UploadGenericRules(UploadGenericRulesInput uploadInput)
        {
            UploadGenericRulesOutput uploadOutput = new UploadGenericRulesOutput();

            string errorMessage = null;
            List<ParsedGenericRuleRow> parsedExcel = null;
            if (!ParseExcel(uploadInput.FileId, out errorMessage, out parsedExcel))
            {
                return new UploadGenericRulesOutput
                {
                    ErrorMessage = errorMessage
                };
            }

            List<GenericRuleRowToAdd> addedGenericRows = new List<GenericRuleRowToAdd>();
            List<GenericRuleInvalidRow> invalidGenericRows = new List<GenericRuleInvalidRow>();
            
            CreateGenericRuleFromExcel(parsedExcel, uploadInput.GenericRuleDefinitionId, invalidGenericRows, addedGenericRows, uploadInput.EffectiveDate, uploadInput.CriteriaFieldsValues);

            List<GenericRule> addedGenericRules = null;
            long outputFileId = 0;
            ReflectGenericRulesToDBAndExcel(addedGenericRows, invalidGenericRows, uploadInput.GenericRuleDefinitionId, uploadInput.EffectiveDate, uploadInput.FileId, out addedGenericRules, out outputFileId);

            return new UploadGenericRulesOutput
            {
                NumberOfGenericRulesAdded = addedGenericRules != null ? addedGenericRules.Count : 0,
                NumberOfGenericRulesFailed = invalidGenericRows != null ? invalidGenericRows.Count : 0,
                FileId = outputFileId
            };
        }

        public byte[] DownloadUploadGenericRulesOutput(long fileId)
        {
            VRFileManager manager = new VRFileManager();
            VRFile file = manager.GetFile(fileId);
            byte[] bytes = file.Content;
            return bytes;
        }

        #endregion

        #region Private Methods

        Dictionary<Guid, GenericRuleDefinition> GetCachedGenericRuleDefinitions()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetGenericRuleDefinitions",
                () =>
                {
                    IGenericRuleDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericRuleDefinitionDataManager>();
                    IEnumerable<GenericRuleDefinition> genericRuleDefinitions = dataManager.GetGenericRuleDefinitions();
                    var dictionary = genericRuleDefinitions.ToDictionary(genericRuleDefinition => genericRuleDefinition.GenericRuleDefinitionId, genericRuleDefinition => genericRuleDefinition);
                    return dictionary;
                });
        }

        private bool DoesUserHaveAccess(RequiredPermissionSettings requiredPermission)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            SecurityManager secManager = new SecurityManager();
            if (!secManager.IsAllowed(requiredPermission, userId))
                return false;
            return true;

        }
        private bool DoesUserHaveAccess(int userId, RequiredPermissionSettings requiredPermission)
        {
            SecurityManager secManager = new SecurityManager();
            if (!secManager.IsAllowed(requiredPermission, userId))
                return false;
            return true;

        }
        private bool ParseExcel(long fileId, out string errorMessage, out List<ParsedGenericRuleRow> parsedExcel)
        {
            VRFileManager fileManager = new VRFileManager();
            byte[] bytes = fileManager.GetFile(fileId).Content;
            var fileStream = new System.IO.MemoryStream(bytes);
            Workbook workbook = new Workbook(fileStream);
            Worksheet worksheet = workbook.Worksheets[0];
            errorMessage = null;
            parsedExcel = new List<ParsedGenericRuleRow>();

            var nbOfRows = worksheet.Cells.Rows.Count;
            var nbOfCols = worksheet.Cells.Columns.Count;
            
            if (nbOfRows == 1)
            {
                errorMessage = "Empty Template";
                return false;
            }

            for (int rowIndex = 1; rowIndex < nbOfRows; rowIndex++)
            {
                ParsedGenericRuleRow parsedRow = new ParsedGenericRuleRow
                {
                    RowIndex = rowIndex,
                    ColumnValueByFieldName = new Dictionary<string, object>()
                };
                for (int colIndex = 0; colIndex < nbOfCols; colIndex++)
                {
                    var header = worksheet.Cells[0, colIndex];
                    if (header.Value == null)
                        continue;
                    
                    var cell = worksheet.Cells[rowIndex, colIndex];

                    if (cell != null)
                    {
                        if (parsedRow.ColumnValueByFieldName.ContainsKey(header.Value.ToString().Trim()))
                        {
                            errorMessage = "Invalid File Format";
                            return false;
                        }
                        else
                        {
                            parsedRow.ColumnValueByFieldName.Add(header.Value.ToString().Trim(), cell.Value);
                        }
                    }

                }
                parsedExcel.Add(parsedRow);
            }

            return true;
        }

        private void CreateGenericRuleFromExcel(List<ParsedGenericRuleRow> parsedExcel, Guid genericRuleDefinitionId, List<GenericRuleInvalidRow> invalidGenericRows, List<GenericRuleRowToAdd> addedGenericRows, DateTime effectiveDate, Dictionary<string, CriteriaFieldsValues> preDefinedValues)
        {
            var genericRuleDefinition = GetGenericRuleDefinition(genericRuleDefinitionId);
            genericRuleDefinition.ThrowIfNull("genericRuleDefinition", genericRuleDefinitionId);
            genericRuleDefinition.SettingsDefinition.ThrowIfNull("genericRuleDefinition.SettingsDefinition", genericRuleDefinitionId);
            if (genericRuleDefinition.SettingsDefinition.SupportUpload == false)
                throw new NotSupportedException("The generic rule does not support bulk uploading");
            genericRuleDefinition.CriteriaDefinition.ThrowIfNull("genericRulesDefinition.CriteriaDefinition", genericRuleDefinitionId);
            genericRuleDefinition.CriteriaDefinition.Fields.ThrowIfNull("genericRuleDefinition.CriteriaDefinition.Fields", genericRuleDefinitionId);


            if (parsedExcel != null && parsedExcel.Count!=0)
            {
                foreach (var parsedRow in parsedExcel)
                {
                    bool isErrorOccured = false;

                    GenericRuleRowToAdd rowToAdd = new GenericRuleRowToAdd
                    {
                        RowIndex = parsedRow.RowIndex,
                        RulesToClose = new List<GenericRule>()
                    };

                    Dictionary<string, object> criteriaByFieldName = new Dictionary<string, object>();
                    Dictionary<string, object> addedFields = new Dictionary<string, object>();
                    Dictionary<string, object> settingFields = new Dictionary<string, object>();
               
                    var criteriaFields = genericRuleDefinition.CriteriaDefinition.Fields;
                    List<AdditionalField> additionalFields = new List<AdditionalField>();

                    foreach (var criteria in criteriaFields)
                    {
                        var cell = parsedRow.ColumnValueByFieldName.GetRecord(criteria.Title);
                        if (cell != null)
                        {
                            var dataRecordContext = new GetValueByDescriptionContext
                            {
                                AdditionalFields = new List<AdditionalField>(),
                            };
                            
                            dataRecordContext.FieldDescription = cell;
                            dataRecordContext.FieldType = criteria.FieldType;
                            criteria.FieldType.GetValueByDescription(dataRecordContext);
                            
                            if (dataRecordContext.ErrorMessage != null)
                            {
                                invalidGenericRows.Add(new GenericRuleInvalidRow
                                {
                                    RowIndex = parsedRow.RowIndex,
                                    ErrorMessage = dataRecordContext.ErrorMessage
                                });
                                isErrorOccured = true;
                                break;
                            }
                            else
                            {
                                criteriaByFieldName.Add(criteria.FieldName, dataRecordContext.FieldValue);
                               
                                additionalFields.Add(new AdditionalField
                                {
                                    FieldType = criteria.FieldType,
                                    FieldValue = dataRecordContext.FieldValue
                                });

                                dataRecordContext.AdditionalFields = additionalFields;
                                addedFields.Add(criteria.Title, dataRecordContext.FieldValue);
                            }
                        }
                    }
                    if (!isErrorOccured)
                    {
                        foreach (var col in parsedRow.ColumnValueByFieldName)
                        {
                            if (addedFields.ContainsKey(col.Key) || col.Key.Equals("description", StringComparison.InvariantCultureIgnoreCase))
                                continue;
                            settingFields.Add(col.Key, col.Value);
                        }
                    }
                    if (settingFields != null && settingFields.Count!=0)
                    {
                        var settingsContext = new CreateGenericRuleFromExcelContext
                        {
                            ParsedGenericRulesFields = settingFields
                        };
                        genericRuleDefinition.SettingsDefinition.CreateGenericRuleFromExcel(settingsContext);
                        if (settingsContext.ErrorMessage != null)
                        {
                            isErrorOccured = true;
                            invalidGenericRows.Add(new GenericRuleInvalidRow
                            {
                                RowIndex = parsedRow.RowIndex,
                                ErrorMessage = settingsContext.ErrorMessage
                            });
                        }
                        else
                        {
                            var genericRule = settingsContext.GenericRule;
                            var descriptionCell = parsedRow.ColumnValueByFieldName.GetRecord("Description");
                            if (descriptionCell != null)
                                genericRule.Description = descriptionCell.ToString().Trim();
                            genericRule.DefinitionId = genericRuleDefinition.GenericRuleDefinitionId;
                            genericRule.BeginEffectiveTime = effectiveDate;
                            genericRule.Criteria = new GenericRuleCriteria
                            {
                                FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>()
                            };
                            IGenericRuleCriteriaManager criteriaManager = GenericDataMainExtensionsFactory.GetManager<IGenericRuleCriteriaManager>();

                            foreach (var criteria in criteriaByFieldName)
                            {
                                var values = criteriaManager.GetCriteriaFieldValues(criteria.Value);
                                genericRule.Criteria.FieldsValues.Add(criteria.Key, values);
                            }
                            if (preDefinedValues != null)
                            {
                                foreach (var preDefinedCriteria in preDefinedValues)
                                {
                                    if (preDefinedCriteria.Value != null && preDefinedCriteria.Value.Values != null)
                                    {
                                        var predefinedCriteriaValue = preDefinedCriteria.Value.Values.First();
                                        if (predefinedCriteriaValue != null)
                                        {
                                            var values = criteriaManager.GetCriteriaFieldValues(predefinedCriteriaValue);
                                            genericRule.Criteria.FieldsValues.Add(preDefinedCriteria.Key, values);
                                        }
                                    }
                                }
                            }

                            if (genericRule != null)
                                rowToAdd.RuleToAdd = genericRule;
                        }
                    }

                    if (!isErrorOccured)
                        addedGenericRows.Add(rowToAdd);
                }
            }
        }
        public void ReflectGenericRulesToDBAndExcel(List<GenericRuleRowToAdd> addedGenericRows, List<GenericRuleInvalidRow> invalidGenericRows, Guid genericRuleDefinitionId, DateTime effectiveDate, long uploadFileId, out List<GenericRule> addedGenericRules, out long outputFileId)
        {
            var genericRuleManager = GetManager(genericRuleDefinitionId);
            var allGenericRules = genericRuleManager.GetGenericRulesByDefinitionId(genericRuleDefinitionId);

            outputFileId = 0;
            addedGenericRules = new List<GenericRule>();

            VRFileManager manager = new VRFileManager();
            VRFile file = manager.GetFile(uploadFileId);
            var fileStream = new System.IO.MemoryStream(file.Content);
            Workbook UploadOutputWorkbook = new Workbook(fileStream);
            Vanrise.Common.Utilities.ActivateAspose();
            Worksheet UploadOutputWorksheet = UploadOutputWorkbook.Worksheets[0];

            UploadOutputWorksheet.Name = "Result";
            int colnum = UploadOutputWorksheet.Cells.Columns.Count;
            UploadOutputWorksheet.Cells.SetColumnWidth(colnum, 20);
            UploadOutputWorksheet.Cells.SetColumnWidth(colnum + 1, 40);
            UploadOutputWorksheet.Cells[0, colnum].PutValue("Result");
            UploadOutputWorksheet.Cells[0, colnum + 1].PutValue("Error Message");

            Style headerStyle = new Style();
            headerStyle.Font.Name = "Times New Roman";
            headerStyle.Font.Color = Color.Red;
            headerStyle.Font.Size = 14;
            headerStyle.Font.IsBold = true;

            UploadOutputWorksheet.Cells[0, colnum].SetStyle(headerStyle);
            UploadOutputWorksheet.Cells[0, colnum + 1].SetStyle(headerStyle);

            Style cellStyle = new Style();
            cellStyle.Font.Name = "Times New Roman";
            cellStyle.Font.Color = Color.Black;
            cellStyle.Font.Size = 12;

            foreach (var genericRow in addedGenericRows)
            {

                foreach (var rule in allGenericRules)
                {
                    if (rule.Criteria != null && rule.Criteria.FieldsValues != null)
                    {
                        if (rule.Criteria.FieldsValues.Count == genericRow.RuleToAdd.Criteria.FieldsValues.Count)
                        {
                            int count = 0;
                            foreach (var fieldValue in rule.Criteria.FieldsValues)
                            {
                                var commonCriteriaExists = genericRow.RuleToAdd.Criteria.FieldsValues.ContainsKey(fieldValue.Key);
                                if (commonCriteriaExists != false)
                                {
                                    var existingRuleValues = rule.Criteria.FieldsValues[fieldValue.Key].GetValues();
                                    var newRuleValues = genericRow.RuleToAdd.Criteria.FieldsValues[fieldValue.Key].GetValues();
                                    var isMatch = false;
                                    if (existingRuleValues != null && newRuleValues != null && newRuleValues.Count() == existingRuleValues.Count())
                                    {
                                        isMatch = true;
                                        foreach (var newRuleValue in newRuleValues)
                                        {
                                            if (!existingRuleValues.Any(x => x.ToString().Equals(newRuleValue.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                                            {
                                                isMatch = false;
                                                break;
                                            }
                                        }
                                        if(isMatch)
                                        {
                                            foreach (var existingRuleValue in existingRuleValues)
                                            {
                                                if (!newRuleValues.Any(x => x.ToString().Equals(existingRuleValue.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                                                {
                                                    isMatch = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    if (isMatch)
                                        count++;
                                }
                            }
                            if (count == genericRow.RuleToAdd.Criteria.FieldsValues.Count)
                            {
                                if (rule.EndEffectiveTime.VRGreaterThan(effectiveDate))
                                {
                                    rule.EndEffectiveTime = Utilities.Max(rule.BeginEffectiveTime, effectiveDate);
                                    genericRow.RulesToClose.Add(rule);
                                }
                            }
                        }
                    }
                }

                var insertOperationOutput = genericRuleManager.AddGenericRule(genericRow.RuleToAdd);
                if (insertOperationOutput.Result == InsertOperationResult.Succeeded)
                {
                    addedGenericRules.Add(genericRow.RuleToAdd);

                    if (genericRow.RulesToClose != null && genericRow.RulesToClose.Count > 0)
                    {
                        foreach (var ruleToClose in genericRow.RulesToClose)
                        {
                            genericRuleManager.UpdateGenericRule(ruleToClose);
                        }
                    }
                    UploadOutputWorksheet.Cells[genericRow.RowIndex, colnum].PutValue("Succeeded.");
                    UploadOutputWorksheet.Cells[genericRow.RowIndex, colnum].SetStyle(cellStyle);
                }
                else
                {
                    invalidGenericRows.Add(new GenericRuleInvalidRow
                    {
                        RowIndex = genericRow.RowIndex,
                        ErrorMessage = string.Format("An error occured while adding rule to database: ", insertOperationOutput.Result.ToString())
                    });
                }
  
            }
            foreach (var genericRowFailed in invalidGenericRows)
            {
                UploadOutputWorksheet.Cells[genericRowFailed.RowIndex, colnum].PutValue("Failed.");
                UploadOutputWorksheet.Cells[genericRowFailed.RowIndex, colnum + 1].PutValue(genericRowFailed.ErrorMessage);
                UploadOutputWorksheet.Cells[genericRowFailed.RowIndex, colnum].SetStyle(cellStyle);
                UploadOutputWorksheet.Cells[genericRowFailed.RowIndex, colnum + 1].SetStyle(cellStyle);
            }
            
            MemoryStream memoryStream = new MemoryStream();
            memoryStream = UploadOutputWorkbook.SaveToStream();

            VRFile returnedFile = new VRFile()
            {
                Content = memoryStream.ToArray(),
                Name = "UploadGenericRulesOutput",
                Extension = ".xlsx",
                IsTemp = true

            };
            VRFileManager fileManager = new VRFileManager();
            outputFileId =  fileManager.AddFile(returnedFile);
        }
        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IGenericRuleDefinitionDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericRuleDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreGenericRuleDefinitionsUpdated(ref _updateHandle);
            }
        }

        private class GenericRuleDefinitionLoggableEntity : VRLoggableEntityBase
        {
            public static GenericRuleDefinitionLoggableEntity Instance = new GenericRuleDefinitionLoggableEntity();

            private GenericRuleDefinitionLoggableEntity()
            {

            }

            static GenericRuleDefinitionManager s_genericRuleDefintionManager = new GenericRuleDefinitionManager();

            public override string EntityUniqueName
            {
                get { return "VR_GenericData_GenericRuleDefinition"; }
            }

            public override string ModuleName
            {
                get { return "Generic Data"; }
            }

            public override string EntityDisplayName
            {
                get { return "Generic Rule Definition"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_GenericData_GenericRuleDefinition_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                GenericRuleDefinition genericRuleDefinition = context.Object.CastWithValidate<GenericRuleDefinition>("context.Object");
                return genericRuleDefinition.GenericRuleDefinitionId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                GenericRuleDefinition genericRuleDefinition = context.Object.CastWithValidate<GenericRuleDefinition>("context.Object");
                return s_genericRuleDefintionManager.GetGenericRuleDefinitionName(genericRuleDefinition.GenericRuleDefinitionId);
            }
        }

        #endregion

        #region Mappers

        public GenericRuleDefinitionInfo GenericRuleDefinitionInfoMapper(GenericRuleDefinition genericRuleDefinition)
        {
            return new GenericRuleDefinitionInfo()
            {
                GenericRuleDefinitionId = genericRuleDefinition.GenericRuleDefinitionId,
                Name = genericRuleDefinition.Name,
                Title = genericRuleDefinition.Title
            };
        }

        #endregion
    }
}
