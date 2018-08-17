using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public interface IDataRecordFiller
    {
        void SetFieldValue(string fieldName, dynamic fieldValue);
        dynamic GetFieldValue(string fieldName);
        Dictionary<string, dynamic> GetDictionaryFromDataRecordType();
        dynamic CloneRecord(Guid dataRecordTypeId);
    }

    public class DataRecordTypeManager : IDataRecordTypeManager
    {
        #region Public Methods

        static CacheManager s_cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>();

        CacheManager GetCacheManager()
        {
            return s_cacheManager;
        }

        public IDataRetrievalResult<DataRecordTypeDetail> GetFilteredDataRecordTypes(DataRetrievalInput<DataRecordTypeQuery> input)
        {
            var allItems = GetCachedDataRecordTypes();

            Func<DataRecordType, bool> filterExpression = (itemObject) =>
                 (input.Query.Name == null || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()));
            VRActionLogger.Current.LogGetFilteredAction(DataRecordTypeLoggableEntity.Instance, input);
            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, DataRecordTypeDetailMapper));
        }

        public DataRecordType GetDataRecordTypeToEdit(Guid dataRecordTypeId)
        {
            var dataRecordTypes = GetCachedDataRecordTypeDefinitions();

            return dataRecordTypes.GetRecord(dataRecordTypeId); ;
        }

        public DataRecordType GetDataRecordType(Guid dataRecordTypeId)
        {
            var dataRecordTypes = GetCachedDataRecordTypes();
            return dataRecordTypes.GetRecord(dataRecordTypeId);
        }
        public string GetIdFieldRuntimeTypeAsString(Guid dataRecordTypeId)
        {
            var dataRecordType = GetDataRecordType(dataRecordTypeId);
            dataRecordType.Settings.IdField.ThrowIfNull("dataRecordType.Settings.IdField");
            var idDataRecordField = dataRecordType.Fields.FindRecord(x => x.Name == dataRecordType.Settings.IdField);
            idDataRecordField.ThrowIfNull("idDataRecordField");
            return CSharpCompiler.TypeToString(idDataRecordField.Type.GetRuntimeType());
        }
        public DataRecordType GetDataRecordType(string dataRecordTypeName)
        {
            var dataRecordTypes = GetCachedDataRecordTypesByName();
            return dataRecordTypes.GetRecord(dataRecordTypeName);
        }

        public string GetDataRecordTypeName(Guid dataRecordTypeId)
        {
            var dataRecordTypes = GetCachedDataRecordTypes();
            DataRecordType dataRecordType = dataRecordTypes.GetRecord(dataRecordTypeId);

            if (dataRecordType != null)
                return dataRecordType.Name;

            return null;
        }

        public List<DataRecordGridColumnAttribute> GetDataRecordAttributes(Guid dataRecordTypeId)
        {
            List<DataRecordGridColumnAttribute> fields = new List<DataRecordGridColumnAttribute>();
            DataRecordType dataRecordType = GetDataRecordType(dataRecordTypeId);
            foreach (DataRecordField field in dataRecordType.Fields)
            {
                DataRecordGridColumnAttribute attribute = new DataRecordGridColumnAttribute() { Attribute = field.Type.GetGridColumnAttribute(null), Name = field.Name, DetailViewerEditor = field.Type.DetailViewerEditor };
                fields.Add(attribute);
            }
            return fields;
        }

        public Dictionary<string, DataRecordField> GetDataRecordTypeFields(Guid dataRecordTypeId)
        {
            string cacheName = String.Format("GetDataRecordTypeFields_{0}", dataRecordTypeId);
            return GetCacheManager().GetOrCreateObject(cacheName,
               () =>
               {
                   var dataRecordType = GetDataRecordType(dataRecordTypeId);
                   if (dataRecordType == null)
                       throw new NullReferenceException(String.Format("dataRecordType '{0}'", dataRecordTypeId));
                   if (dataRecordType.Fields == null)
                       throw new NullReferenceException(String.Format("dataRecordType.Fields '{0}'", dataRecordTypeId));
                   return dataRecordType.Fields.ToDictionary(itm => itm.Name, itm => itm);
               });
        }
        public Dictionary<string, Object> ParseDicValuesToFieldType(Guid dataRecordTypeId, Dictionary<string, Object> fieldValues)
        {
            Dictionary<string, Object> parsedDicValues = null;
            if (fieldValues != null)
            {
                var dataRecordFields = GetDataRecordTypeFields(dataRecordTypeId);
                dataRecordFields.ThrowIfNull("dataRecordFields", dataRecordTypeId);
                parsedDicValues = new Dictionary<string, object>();
                foreach (var fieldValue in fieldValues)
                {
                    var dataRecordField = dataRecordFields.GetRecord(fieldValue.Key);
                    dataRecordField.ThrowIfNull("dataRecordField", String.Format(" {0}: {1}", dataRecordTypeId, fieldValue.Key));
                    parsedDicValues.Add(fieldValue.Key, dataRecordField.Type.ParseValueToFieldType(new DataRecordFieldTypeParseValueToFieldTypeContext(fieldValue.Value)));
                }
            }
            return parsedDicValues;
        }
        public DataRecordField GetDataRecordField(Guid dataRecordTypeId, string fieldName)
        {
            var dataRecordFields = GetDataRecordTypeFields(dataRecordTypeId);
            DataRecordField dataRecordField = null;
            if (dataRecordFields != null)
            {
                dataRecordFields.TryGetValue(fieldName, out dataRecordField);
            }
            return dataRecordField;
        }

        public IEnumerable<DataRecordTypeInfo> GetDataRecordTypeInfo(DataRecordTypeInfoFilter filter)
        {
            var dataRecordTypes = GetCachedDataRecordTypes();
            if (filter != null)
            {
                Func<DataRecordType, bool> filterExpression = (x) => (filter.RecordTypeIds.Contains(x.DataRecordTypeId));
                return dataRecordTypes.FindAllRecords(filterExpression).MapRecords(DataRecordTypeInfoMapper);
            }
            else
            {
                return dataRecordTypes.MapRecords(DataRecordTypeInfoMapper);
            }
        }

        public IEnumerable<DataRecordTypeInfo> GetRemoteDataRecordTypeInfo(Guid connectionId, string serializedFilter)
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

            return connectionSettings.Get<IEnumerable<DataRecordTypeInfo>>(string.Format("/api/VR_GenericData/DataRecordType/GetDataRecordTypeInfo?serializedFilter={0}", serializedFilter));
        }

        public Vanrise.Entities.InsertOperationOutput<DataRecordTypeDetail> AddDataRecordType(DataRecordType dataRecordType)
        {
            InsertOperationOutput<DataRecordTypeDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<DataRecordTypeDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            dataRecordType.DataRecordTypeId = Guid.NewGuid();

            IDataRecordTypeDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordTypeDataManager>();
            bool insertActionSucc = dataManager.AddDataRecordType(dataRecordType);

            if (insertActionSucc)
            {
                GetCacheManager().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(DataRecordTypeLoggableEntity.Instance, dataRecordType);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = DataRecordTypeDetailMapper(dataRecordType);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<DataRecordTypeDetail> UpdateDataRecordType(DataRecordType dataRecordType)
        {
            IDataRecordTypeDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordTypeDataManager>();
            bool updateActionSucc = dataManager.UpdateDataRecordType(dataRecordType);
            UpdateOperationOutput<DataRecordTypeDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<DataRecordTypeDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                GetCacheManager().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(DataRecordTypeLoggableEntity.Instance, dataRecordType);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = DataRecordTypeDetailMapper(dataRecordType);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public dynamic CreateDataRecordObject(string dataRecordTypeName)
        {
            Type dataRecordRuntimeType = GetDataRecordRuntimeType(dataRecordTypeName);
            if (dataRecordRuntimeType != null)
                return Activator.CreateInstance(dataRecordRuntimeType);
            return null;
        }

        public Type GetDataRecordRuntimeType(Guid dataRecordTypeId)
        {
            string cacheName = String.Format("GetDataRecordRuntimeTypeById_{0}", dataRecordTypeId);
            var runtimeType = GetCacheManager().GetOrCreateObject(cacheName,
                () =>
                {
                    DataRecordType dataRecordType = GetCachedDataRecordTypes().GetRecord(dataRecordTypeId);
                    if (dataRecordType != null)
                        return GetOrCreateRuntimeType(dataRecordType);
                    else
                        return null;
                });
            if (runtimeType == null)
                throw new ArgumentException(String.Format("Cannot create runtime type from Data Record Type Id '{0}'", dataRecordTypeId));
            return runtimeType;
        }

        public Type GetDataRecordRuntimeType(string dataRecordTypeName)
        {
            string cacheName = String.Format("GetDataRecordRuntimeTypeByName_{0}", dataRecordTypeName);
            var runtimeType = GetCacheManager().GetOrCreateObject(cacheName,
                () =>
                {
                    DataRecordType dataRecordType = GetCachedDataRecordTypes().FindRecord(itm => itm.Name == dataRecordTypeName);
                    if (dataRecordType != null)
                        return GetDataRecordRuntimeType(dataRecordType.DataRecordTypeId);
                    else
                        return null;
                });
            if (runtimeType == null)
                throw new ArgumentException(String.Format("Cannot create runtime type from Data Record Type Name '{0}'", dataRecordTypeName));
            return runtimeType;
        }

        public dynamic ConvertDynamicToDataRecord(dynamic dynamicObject, Guid dataRecordTypeId)
        {
            return Serializer.Deserialize(SerializeRecord(dynamicObject, dataRecordTypeId), GetDataRecordRuntimeType(dataRecordTypeId));
        }

        public string SerializeRecord(dynamic record, Guid dataRecordTypeId)
        {
            return Serializer.Serialize(record, true);
        }

        public dynamic DeserializeRecord(string serializedRecord, Guid dataRecordTypeId)
        {
            return Serializer.Deserialize(serializedRecord, GetDataRecordRuntimeType(dataRecordTypeId));
        }

        public IDataRecordFieldEvaluator GetFieldEvaluator(Guid dataRecordTypeId)
        {
            string cacheName = String.Format("GetFieldEvaluator_{0}", dataRecordTypeId);
            return GetCacheManager().GetOrCreateObject(cacheName,
                () =>
                {
                    var fields = GetDataRecordTypeFields(dataRecordTypeId);
                    if (fields == null)
                        throw new NullReferenceException(String.Format("fields. dataRecordTypeId '{0}'", dataRecordTypeId));
                    return BuildFieldEvaluator(fields.ToDictionary(itm => itm.Key, itm => itm.Value.Type));
                });
        }

        public IDataRecordFieldEvaluator BuildFieldEvaluator(Dictionary<string, DataRecordFieldType> fieldsByName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataRecordTypeExtraFieldTemplate> GetDataRecordTypeExtraFieldsTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<DataRecordTypeExtraFieldTemplate>(DataRecordTypeExtraFieldTemplate.EXTENSION_TYPE);
        }

        #endregion

        #region Private Methods

        public Dictionary<Guid, DataRecordType> GetCachedDataRecordTypeDefinitions()
        {
            return GetCacheManager().GetOrCreateObject("GetDataRecordTypeDefinitions",
               () =>
               {
                   IDataRecordTypeDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordTypeDataManager>();
                   IEnumerable<DataRecordType> dataRecordTypes = dataManager.GetDataRecordTypes();
                   return dataRecordTypes.ToDictionary(kvp => kvp.DataRecordTypeId, kvp => kvp);
               });
        }

        public Dictionary<Guid, DataRecordType> GetCachedDataRecordTypes()
        {
            return GetCacheManager().GetOrCreateObject("GetDataRecordTypes",
               () =>
               {
                   Dictionary<Guid, DataRecordType> dataRecordTypes = new Dictionary<Guid, DataRecordType>();
                   var dataRecordTypeDefinitions = GetCachedDataRecordTypeDefinitions();
                   StringBuilder strBuilder = new StringBuilder();
                   foreach (var dataRecordTypeDefinition in dataRecordTypeDefinitions)
                   {
                       List<DataRecordField> dataRecordFields = new List<DataRecordField>();
                       DataRecordType dataRecordType = Vanrise.Common.Serializer.Deserialize<DataRecordType>(Vanrise.Common.Serializer.Serialize(dataRecordTypeDefinition.Value));

                       if (dataRecordType.Fields != null)
                           dataRecordFields.AddRange(dataRecordType.Fields);

                       if (dataRecordType.ExtraFieldsEvaluator != null)
                       {
                           var extraFields = dataRecordType.ExtraFieldsEvaluator.GetFields(null);
                           if (extraFields != null)
                           {
                               extraFields = extraFields.VRDeepCopy();
                               extraFields.ForEach((fld) => fld.IsInheritedFromExtraField = true);
                               dataRecordFields.AddRange(extraFields);
                           }
                       }
                       if (dataRecordFields.Count == 0)
                           throw new Exception(String.Format("dataRecordType '{0}' doesn't have any Field", dataRecordType.DataRecordTypeId));

                       dataRecordType.Fields = dataRecordFields;
                       dataRecordTypes.Add(dataRecordType.DataRecordTypeId, dataRecordType);
                   }
                   return dataRecordTypes;
               });
        }

        private Dictionary<string, DataRecordType> GetCachedDataRecordTypesByName()
        {
            return GetCacheManager().GetOrCreateObject("GetDataRecordTypesByName",
               () =>
               {
                   Dictionary<Guid, DataRecordType> dataRecordTypes = GetCachedDataRecordTypes();
                   return dataRecordTypes.ToDictionary(itm => itm.Value.Name, itm => itm.Value);
               });
        }

        private Type GetOrCreateRuntimeType(DataRecordType dataRecordType)
        {
            string fullTypeName;
            var classDefinition = BuildClassDefinition(dataRecordType, out fullTypeName);

            CSharpCompilationOutput compilationOutput;
            if (!CSharpCompiler.TryCompileClass(String.Format("DataRecordType_{0}", dataRecordType.Name), classDefinition, out compilationOutput))
            {
                StringBuilder errorsBuilder = new StringBuilder();
                if (compilationOutput.ErrorMessages != null)
                {
                    foreach (var errorMessage in compilationOutput.ErrorMessages)
                    {
                        errorsBuilder.AppendLine(errorMessage);
                    }
                }

                throw new Exception(String.Format("Compile Error when building Data Record Type '{0}'. Errors: {1}", dataRecordType.Name, errorsBuilder));
            }
            else
                return compilationOutput.OutputAssembly.GetType(fullTypeName);
        }

        private string BuildClassDefinition(DataRecordType dataRecordType, out string fullTypeName)
        {
            StringBuilder classDefinitionBuilder = new StringBuilder(@" 
                using System;
                using System.Collections.Generic;
                using System.IO;
                using System.Data;
                using System.Linq;
                using Vanrise.Common;
                using Vanrise.GenericData.Business;
                using Vanrise.GenericData.Entities;

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : Vanrise.GenericData.Business.IDataRecordFiller
                    {                   
                        static #CLASSNAME#()
                        {
                             Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(#CLASSNAME#) #PROPERTIESTOSETSERIALIZED#);
                             var dummyTime = new Vanrise.Entities.Time(); //this is only to try declaring the ProtoBuf Serialization in the static constructor of the Time Type
                             var recordType = new DataRecordTypeManager().GetDataRecordType(new Guid(""#RECORDTYPEID#""));
                             s_dataRecordFieldDict = recordType.Fields.ToDictionary(itm => itm.Name, itm => itm);  
                             s_dataRecordFieldTypeDict = recordType.Fields.ToDictionary(itm => itm.Name, itm => itm.Type);
                             BuildSetFieldValueActions();
                             BuildGetFieldValueActions();
                        }   
  
                        private static Dictionary<string, DataRecordField> s_dataRecordFieldDict;
                        private static Dictionary<string, DataRecordFieldType> s_dataRecordFieldTypeDict;
                        private static Dictionary<string, Action<#NAMESPACE#.#CLASSNAME#, dynamic>> s_setFieldValueActions;
                        private static Dictionary<string, Func<#NAMESPACE#.#CLASSNAME#, dynamic>> s_getFieldValueActions;


                        public #CLASSNAME#()
                        {
                        }

                        public #CLASSNAME#(Dictionary<string, dynamic> sourceDictionary, dynamic sourceObject)
                            : this()
                        {
                            _sourceDictionary = sourceDictionary;
                            _sourceObject = sourceObject;
                        }

                        public #CLASSNAME#(Dictionary<string, dynamic> sourceDictionary)
                            : this()
                        {
                            _sourceDictionary = sourceDictionary;
                        }

                        public #CLASSNAME#(dynamic sourceObject) 
                            : this()
                        {
                            _sourceObject = sourceObject;
                        }

                        private dynamic _sourceObject;
                        private Dictionary<string, dynamic> _sourceDictionary;

                        public long QueueItemId { get; set; }

                        private static void BuildSetFieldValueActions()
                        {
                            s_setFieldValueActions = new Dictionary<string, Action<#NAMESPACE#.#CLASSNAME#, dynamic>>(#FIELDCOUNT#);
                            #BUILDSETFIELDVALUEACTIONS#
                        }

                        private static void BuildGetFieldValueActions()
                        {
                            s_getFieldValueActions = new Dictionary<string, Func<#NAMESPACE#.#CLASSNAME#, dynamic>>(#FIELDCOUNT#);
                            #BUILDGETFIELDVALUEACTIONS#
                        }

                        #GLOBALMEMBERS#

                        public void SetFieldValue(string fieldName, dynamic fieldValue)
                        {
                            Action<#NAMESPACE#.#CLASSNAME#, dynamic> setFieldValueAction;
                            if(!s_setFieldValueActions.TryGetValue(fieldName, out setFieldValueAction))
                                throw new ArgumentException(String.Format(""fieldName '{0}'"", fieldName), ""fieldName"");
                            setFieldValueAction(this, fieldValue);
                        } 

                        public dynamic GetFieldValue(string fieldName)
                        {
                            Func<#NAMESPACE#.#CLASSNAME#, dynamic> getFieldValueAction;
                            if(!s_getFieldValueActions.TryGetValue(fieldName, out getFieldValueAction))
                                throw new ArgumentException(String.Format(""fieldName '{0}'"", fieldName), ""fieldName"");
                            return getFieldValueAction(this);
                        }

                        public Dictionary<string, dynamic> GetDictionaryFromDataRecordType()
                        {
                            Dictionary<string, dynamic> dic = new Dictionary<string, dynamic>(#FIELDCOUNT#);

                            #FILLDICTIONARYFROMDATARECORD#

                            return dic;
                        }
                    
                        public dynamic CloneRecord(Guid dataRecordTypeId)
                        {
                            dynamic record = new #NAMESPACE#.#CLASSNAME#();
                            #CLONERECORDMEMBERS#
                            return record;
                        }
                    }
                }");

            StringBuilder propertiesToSetSerializedBuilder = new StringBuilder();
            StringBuilder globalMembersDefinitionBuilder = new StringBuilder();

            StringBuilder setFieldValueActionsBuilder = new StringBuilder();
            StringBuilder getFieldValueActionsBuilder = new StringBuilder();

            StringBuilder fillDictionaryFromDataRecordBuilder = new StringBuilder();

            StringBuilder cloneRecordMembersBuilder = new StringBuilder();

            int fieldCount = dataRecordType.Fields.Count;

            foreach (var field in dataRecordType.Fields)
            {
                string fieldRuntimeTypeAsString = CSharpCompiler.TypeToString(field.Type.GetRuntimeType());

                globalMembersDefinitionBuilder.AppendLine(GetGlobalMemberDefinitionScript(fieldRuntimeTypeAsString, field));

                getFieldValueActionsBuilder.AppendFormat(@"s_getFieldValueActions.Add(""{0}"", (dataRecord) => dataRecord.{0});", field.Name);
                getFieldValueActionsBuilder.AppendLine();

                fillDictionaryFromDataRecordBuilder.AppendFormat(@"dic.Add(""{0}"", this.{0});", field.Name);
                fillDictionaryFromDataRecordBuilder.AppendLine();
                StringBuilder addSetFieldValueActionBuilder;
                if (field.Formula == null)
                {

                    propertiesToSetSerializedBuilder.AppendFormat(", \"{0}\"", field.Name);
                    addSetFieldValueActionBuilder = new StringBuilder(@"s_setFieldValueActions.Add(""#FIELDNAME#"", (dataRecord, fieldValue) => 
                                                                                                                    {
                                                                                                                        if(fieldValue == null)
                                                                                                                            dataRecord.#FIELDNAME# = default(#FIELDTYPE#);
                                                                                                                        else if(fieldValue.GetType() == typeof(#FIELDTYPE#))
                                                                                                                            dataRecord.#FIELDNAME# = fieldValue;
                                                                                                                        else
                                                                                                                            dataRecord.#FIELDNAME# = s_dataRecordFieldTypeDict.GetRecord(""#FIELDNAME#"").ParseValueToFieldType(new Vanrise.GenericData.Business.DataRecordFieldTypeParseValueToFieldTypeContext(fieldValue)); 
                                                                                                                    });");

                    cloneRecordMembersBuilder.AppendFormat("record.{0} = this.{0};", field.Name);
                    cloneRecordMembersBuilder.AppendLine();
                }
                else//if the field is Formula
                {
                    addSetFieldValueActionBuilder = new StringBuilder(@"s_setFieldValueActions.Add(""#FIELDNAME#"", (dataRecord, fieldValue) => { });");
                }
                addSetFieldValueActionBuilder.Replace("#FIELDNAME#", field.Name);
                addSetFieldValueActionBuilder.Replace("#FIELDTYPE#", fieldRuntimeTypeAsString);
                setFieldValueActionsBuilder.AppendLine(addSetFieldValueActionBuilder.ToString());
            }

            classDefinitionBuilder.Replace("#GLOBALMEMBERS#", globalMembersDefinitionBuilder.ToString());
            classDefinitionBuilder.Replace("#PROPERTIESTOSETSERIALIZED#", propertiesToSetSerializedBuilder.ToString());
            classDefinitionBuilder.Replace("#BUILDSETFIELDVALUEACTIONS#", setFieldValueActionsBuilder.ToString());
            classDefinitionBuilder.Replace("#BUILDGETFIELDVALUEACTIONS#", getFieldValueActionsBuilder.ToString());
            classDefinitionBuilder.Replace("#FILLDICTIONARYFROMDATARECORD#", fillDictionaryFromDataRecordBuilder.ToString());
            classDefinitionBuilder.Replace("#CLONERECORDMEMBERS#", cloneRecordMembersBuilder.ToString());
            classDefinitionBuilder.Replace("#FIELDCOUNT#", fieldCount.ToString());

            classDefinitionBuilder.Replace("#RECORDTYPEID#", dataRecordType.DataRecordTypeId.ToString());

            string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.GenericData.Runtime");
            string className = "DataRecord";
            classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
            classDefinitionBuilder.Replace("#CLASSNAME#", className);
            fullTypeName = String.Format("{0}.{1}", classNamespace, className);

            return classDefinitionBuilder.ToString();
        }

        private string GetGlobalMemberDefinitionScript(string fieldRuntimeTypeAsString, DataRecordField dataRecordField)
        {
            StringBuilder globalMemberDefinitionBuilder = new StringBuilder();

            string isFieldFilledVariableName = string.Format("_is{0}Filled", dataRecordField.Name);
            string _fieldVariableName = string.Format("_{0}", this.ToLowerFirstChar(dataRecordField.Name));

            globalMemberDefinitionBuilder.AppendLine();
            globalMemberDefinitionBuilder.Append(string.Format("private bool {0}; \n", isFieldFilledVariableName));
            globalMemberDefinitionBuilder.Append(string.Format("private {0} {1};", fieldRuntimeTypeAsString, _fieldVariableName));


            if (dataRecordField.Formula == null)
            {
                globalMemberDefinitionBuilder.Append(@"
                        public #FIELDRUNTIMETYPE# #FIELDNAME#
                        {
                            get
                            {
                                if (#PRIVATEFIELDNAME# == default(#FIELDRUNTIMETYPE#) && !#ISFIELDFILLED#)
                                {
                                    dynamic fldValue = null;
                                    if (_sourceDictionary == null || !_sourceDictionary.TryGetValue(""#FIELDNAME#"", out fldValue))
                                    {
                                        if (_sourceObject != null)
                                            fldValue = _sourceObject.#FIELDNAME#;
                                    }

                                    if(fldValue != null)
                                    {
                                        if(fldValue.GetType() == typeof(#FIELDRUNTIMETYPE#))
                                            #PRIVATEFIELDNAME# = fldValue;
                                        else
                                            #PRIVATEFIELDNAME# = s_dataRecordFieldTypeDict.GetRecord(""#FIELDNAME#"").ParseValueToFieldType(new Vanrise.GenericData.Business.DataRecordFieldTypeParseValueToFieldTypeContext(fldValue));
                                    }

                                    #ISFIELDFILLED# = true;
                                }
                                return #PRIVATEFIELDNAME#;
                            }
                            set
                            {
                                #PRIVATEFIELDNAME# = value;
                            }
                        }");
            }
            else
            {
                globalMemberDefinitionBuilder.Append(@"
                        public #FIELDRUNTIMETYPE# #FIELDNAME#
                        {
                            get
                            {
                                if (!#ISFIELDFILLED#)
                                {
                                    var field = s_dataRecordFieldDict.GetRecord(""#FIELDNAME#"");
                                    var fieldFormulaCalculateValueContext = new DataRecordTypeFieldFormulaCalculateValueContext(s_dataRecordFieldTypeDict, this.GetFieldValue, field.Type);
                                    #PRIVATEFIELDNAME# = field.Formula.CalculateValue(fieldFormulaCalculateValueContext);
                                    #ISFIELDFILLED# = true;
                                }
                                return #PRIVATEFIELDNAME#;
                            }
                        }");
            }
            globalMemberDefinitionBuilder.AppendLine();

            globalMemberDefinitionBuilder.Replace("#FIELDRUNTIMETYPE#", fieldRuntimeTypeAsString);
            globalMemberDefinitionBuilder.Replace("#FIELDNAME#", dataRecordField.Name);
            globalMemberDefinitionBuilder.Replace("#ISFIELDFILLED#", isFieldFilledVariableName);
            globalMemberDefinitionBuilder.Replace("#PRIVATEFIELDNAME#", _fieldVariableName);

            return globalMemberDefinitionBuilder.ToString();
        }

        private string ToLowerFirstChar(string input)
        {
            string newString = input;
            if (!String.IsNullOrEmpty(newString) && Char.IsUpper(newString[0]))
                newString = Char.ToLower(newString[0]) + newString.Substring(1);
            return newString;
        }

        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDataRecordTypeDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreDataRecordTypeUpdated(ref _updateHandle);
            }

            public static void TriggerCacheExpiration()
            {
                IDataRecordTypeDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordTypeDataManager>();
                dataManager.SetDataRecordTypeCacheExpired();
            }
        }

        private class DataRecordTypeLoggableEntity : VRLoggableEntityBase
        {
            public static DataRecordTypeLoggableEntity Instance = new DataRecordTypeLoggableEntity();

            private DataRecordTypeLoggableEntity()
            {

            }

            static DataRecordTypeManager s_dataRecordTypeManager = new DataRecordTypeManager();

            public override string EntityUniqueName
            {
                get { return "VR_GenericData_DataRecordType"; }
            }

            public override string ModuleName
            {
                get { return "Generic Data"; }
            }

            public override string EntityDisplayName
            {
                get { return "Data Record Type"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_GenericData_DataRecordType_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                DataRecordType dataRecordType = context.Object.CastWithValidate<DataRecordType>("context.Object");
                return dataRecordType.DataRecordTypeId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                DataRecordType dataRecordType = context.Object.CastWithValidate<DataRecordType>("context.Object");
                return s_dataRecordTypeManager.GetDataRecordTypeName(dataRecordType.DataRecordTypeId);
            }
        }

        #endregion

        #region Mappers

        private DataRecordTypeDetail DataRecordTypeDetailMapper(DataRecordType dataRecordTypeObject)
        {
            DataRecordTypeDetail dataRecordTypeDetail = new DataRecordTypeDetail();
            dataRecordTypeDetail.Entity = dataRecordTypeObject;
            if (dataRecordTypeObject.ParentId != null)
                dataRecordTypeDetail.ParentName = GetDataRecordTypeName((Guid)dataRecordTypeObject.ParentId);
            return dataRecordTypeDetail;
        }

        private DataRecordTypeInfo DataRecordTypeInfoMapper(DataRecordType dataRecordTypeObject)
        {
            DataRecordTypeInfo dataRecordTypeInfo = new DataRecordTypeInfo();
            dataRecordTypeInfo.DataRecordTypeId = dataRecordTypeObject.DataRecordTypeId;
            dataRecordTypeInfo.Name = dataRecordTypeObject.Name;
            return dataRecordTypeInfo;
        }

        #endregion
    }

    public class RecordTypeFieldsPropValueCompilationStep : IPropValueReaderCompilationStep
    {
        public HashSet<string> GetPropertiesToCompile(IPropValueReaderCompilationStepContext context)
        {
            HashSet<string> properties = new HashSet<string>();
            var manager = new DataRecordTypeManager();
            var allRecordTypes = manager.GetCachedDataRecordTypes();
            if (allRecordTypes != null)
            {
                foreach (var recordType in allRecordTypes.Values)
                {
                    var fields = manager.GetDataRecordTypeFields(recordType.DataRecordTypeId);
                    if (fields != null)
                    {
                        foreach (var fld in fields.Values)
                        {
                            properties.Add(fld.Name);
                        }
                    }
                }
            }
            return properties;
        }
    }
}
