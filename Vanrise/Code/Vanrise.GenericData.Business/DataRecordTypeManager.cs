using System;
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
        void FillDataRecordTypeFromDictionary(Dictionary<string, dynamic> source);
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
                               dataRecordFields.AddRange(extraFields);
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
                throw new Exception(String.Format("Compile Error when building Data Record Type '{0}'. Errors: {1}",
                    dataRecordType.Name, errorsBuilder));
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
                        }   
  
                        DataRecordType recordType = new DataRecordTypeManager().GetDataRecordType(new Guid(""#RECORDTYPEID#""));

                        private Dictionary<string, DataRecordField> _dataRecordFieldDict;

                        private Dictionary<string, DataRecordField> DataRecordFieldDict
                        {
                            get
                            {
                                if (_dataRecordFieldDict == null)
                                {
                                    _dataRecordFieldDict = recordType.Fields.ToDictionary(itm => itm.Name, itm => itm);
                                }
                                return _dataRecordFieldDict;
                            }
                        }

                        private Dictionary<string, DataRecordFieldType> _dataRecordFieldTypeDict;

                        private Dictionary<string, DataRecordFieldType> DataRecordFieldTypeDict
                        {
                            get
                            {
                                if (_dataRecordFieldTypeDict == null)
                                {   
                                    _dataRecordFieldTypeDict = recordType.Fields.ToDictionary(itm => itm.Name, itm => itm.Type);
                                }
                                return _dataRecordFieldTypeDict;
                            }
                        }

                        public long QueueItemId { get; set; }

                        #GLOBALMEMBERS#

                        public void SetFieldValue(string fieldName, dynamic fieldValue)
                        {
                            switch(fieldName)
                            {
                                #SETFIELDMEMBERS#
                                default : break;
                            }
                        } 

                        public dynamic GetFieldValue(string fieldName)
                        {
                            switch(fieldName)
                            {
                                #GETFIELDMEMBERS#
                                default : break;
                            }
                            return null;
                        }

                        public void FillDataRecordTypeFromDictionary(Dictionary<string, dynamic> source)
                        {
                            foreach(var item in source)
                            {
                                SetFieldValue(item.Key, item.Value);
                            }
                        }

                        public Dictionary<string, dynamic> GetDictionaryFromDataRecordType()
                        {
                            Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();

                            List<string> fieldNames =  new List<string>() { #FIELDNAMES# };

                            foreach(var fieldName in fieldNames)
                            {
                                if (!results.ContainsKey(fieldName))
                                    results.Add(fieldName, GetFieldValue(fieldName));
                            }

                            return results;
                        }
                    
                        public dynamic CloneRecord(Guid dataRecordTypeId)
                        {
                            Vanrise.GenericData.Business.DataRecordTypeManager dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
                            dynamic record = Activator.CreateInstance(dataRecordTypeManager.GetDataRecordRuntimeType(dataRecordTypeId));
                            #CLONERECORDMEMBERS#
                            return record;
                        }
                    }
                }");

            StringBuilder propertiesToSetSerializedBuilder = new StringBuilder();
            StringBuilder globalMembersDefinitionBuilder = new StringBuilder();

            StringBuilder setFieldValueBuilder = new StringBuilder();
            StringBuilder getFieldValueBuilder = new StringBuilder();

            StringBuilder cloneRecordMembersBuilder = new StringBuilder();

            List<string> fieldNames = new List<string>();
            foreach (var field in dataRecordType.Fields)
            {
                string fieldRuntimeTypeAsString = CSharpCompiler.TypeToString(field.Type.GetRuntimeType());

                globalMembersDefinitionBuilder.AppendLine(GetGlobalMemberDefinitionScript(fieldRuntimeTypeAsString, field));
                getFieldValueBuilder.AppendFormat(@"case ""{0}"" : return {0};", field.Name);

                if (field.Formula == null)
                {
                    propertiesToSetSerializedBuilder.AppendFormat(", \"{0}\"", field.Name);
                    setFieldValueBuilder.AppendFormat(@"case ""{0}"" : if(fieldValue != null) {0} = ({1})Convert.ChangeType(fieldValue, typeof({1})); else {0} = default({2}); break;", field.Name, CSharpCompiler.TypeToString(field.Type.GetNonNullableRuntimeType()), fieldRuntimeTypeAsString);
                    cloneRecordMembersBuilder.AppendFormat("record.{0} = this.{0};", field.Name);
                }

                fieldNames.Add(field.Name);
            }

            classDefinitionBuilder.Replace("#GLOBALMEMBERS#", globalMembersDefinitionBuilder.ToString());
            classDefinitionBuilder.Replace("#PROPERTIESTOSETSERIALIZED#", propertiesToSetSerializedBuilder.ToString());
            classDefinitionBuilder.Replace("#SETFIELDMEMBERS#", setFieldValueBuilder.ToString());
            classDefinitionBuilder.Replace("#GETFIELDMEMBERS#", getFieldValueBuilder.ToString());
            classDefinitionBuilder.Replace("#CLONERECORDMEMBERS#", cloneRecordMembersBuilder.ToString());

            string fieldNamesAsString = string.Format(@"""{0}""", string.Join<string>(@""",""", fieldNames));
            classDefinitionBuilder.Replace("#FIELDNAMES#", fieldNamesAsString);

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
            if (dataRecordField.Formula == null)
            {
                return string.Format("public {0} {1} {{ get; set; }}", fieldRuntimeTypeAsString, dataRecordField.Name);
            }
            else
            {
                StringBuilder globalMemberDefinitionBuilder = new StringBuilder();

                string isFieldFilled = string.Format("_is{0}Filled", dataRecordField.Name);
                string _fieldName = string.Format("_{0}", this.ToLowerFirstChar(dataRecordField.Name));

                globalMemberDefinitionBuilder.AppendLine();
                globalMemberDefinitionBuilder.Append(string.Format("private bool {0}; \n", isFieldFilled));
                globalMemberDefinitionBuilder.Append(string.Format("private {0} {1};", fieldRuntimeTypeAsString, _fieldName));

                globalMemberDefinitionBuilder.Append(@"
                        public #FIELDRUNTIMETYPE# #FIELDNAME#
                        {
                            get
                            {
                                if (#ISFIELDFILLED#)
                                {
                                    return #PRIVATEFIELDNAME#;
                                }
                                else
                                {
                                    var fieldFormulaCalculateValueContext = new DataRecordTypeFieldFormulaCalculateValueContext(DataRecordFieldTypeDict, this.GetFieldValue, DataRecordFieldTypeDict.GetRecord(""#FIELDNAME#""));
                                    #PRIVATEFIELDNAME# = DataRecordFieldDict.GetRecord(""#FIELDNAME#"").Formula.CalculateValue(fieldFormulaCalculateValueContext);
                                    #ISFIELDFILLED# = true;
                                    return #PRIVATEFIELDNAME#;
                                }
                            }
                        }");
                globalMemberDefinitionBuilder.AppendLine();

                globalMemberDefinitionBuilder.Replace("#FIELDRUNTIMETYPE#", fieldRuntimeTypeAsString);
                globalMemberDefinitionBuilder.Replace("#FIELDNAME#", dataRecordField.Name);
                globalMemberDefinitionBuilder.Replace("#ISFIELDFILLED#", isFieldFilled);
                globalMemberDefinitionBuilder.Replace("#PRIVATEFIELDNAME#", _fieldName);

                return globalMemberDefinitionBuilder.ToString();
            }
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
