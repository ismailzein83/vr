using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Caching;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;
namespace Vanrise.GenericData.Business
{
    public interface IDataRecordFiller
    {
        void FillDataRecordTypeFromDictionary(Dictionary<string, dynamic> source);
        void SetFieldValue(string fieldName, dynamic fieldValue);
        dynamic GetFieldValue(string fieldName);
    }

    public class DataRecordTypeManager : IDataRecordTypeManager
    {
        #region Public Methods
        public IDataRetrievalResult<DataRecordTypeDetail> GetFilteredDataRecordTypes(DataRetrievalInput<DataRecordTypeQuery> input)
        {
            var allItems = GetCachedDataRecordTypes();

            Func<DataRecordType, bool> filterExpression = (itemObject) =>
                 (input.Query.Name == null || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, DataRecordTypeDetailMapper));
        }
        public DataRecordType GetDataRecordTypeToEdit(Guid dataRecordTypeId)
        {
            var dataRecordTypes = GetCachedDataRecordTypeDefinitions();
            return dataRecordTypes.GetRecord(dataRecordTypeId);
        }

        public DataRecordType GetDataRecordType(Guid dataRecordTypeId)
        {
            var dataRecordTypes = GetCachedDataRecordTypes();
            return dataRecordTypes.GetRecord(dataRecordTypeId);
        }

        public List<DataRecordGridColumnAttribute> GetDataRecordAttributes(Guid dataRecordTypeId)
        {
            List<DataRecordGridColumnAttribute> fields = new List<DataRecordGridColumnAttribute>();
            DataRecordType dataRecordType = GetDataRecordType(dataRecordTypeId);
            foreach (DataRecordField field in dataRecordType.Fields)
            {
                DataRecordGridColumnAttribute attribute = new DataRecordGridColumnAttribute() { Attribute = field.Type.GetGridColumnAttribute(null), Name = field.Name };
                fields.Add(attribute);
            }
            return fields;
        }
        public Dictionary<string, DataRecordField> GetDataRecordTypeFields(Guid dataRecordTypeId)
        {
            string cacheName = String.Format("GetDataRecordTypeFields_{0}", dataRecordTypeId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
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

        public string GetDataRecordTypeName(Guid dataRecordTypeId)
        {
            var dataRecordTypes = GetCachedDataRecordTypes();
            DataRecordType dataRecordType = dataRecordTypes.GetRecord(dataRecordTypeId);

            if (dataRecordType != null)
                return dataRecordType.Name;

            return null;
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
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
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
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
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
            var runtimeType = CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
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
            var runtimeType = CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
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
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
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
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDataRecordTypeDefinitions",
               () =>
               {
                   IDataRecordTypeDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordTypeDataManager>();
                   IEnumerable<DataRecordType> dataRecordTypes = dataManager.GetDataRecordTypes();
                   return dataRecordTypes.ToDictionary(kvp => kvp.DataRecordTypeId, kvp => kvp);
               });
        }

        public Dictionary<Guid, DataRecordType> GetCachedDataRecordTypes()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDataRecordTypes",
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

        private Type GetOrCreateRuntimeType(DataRecordType dataRecordType)
        {
            string fullTypeName;
            var classDefinition = BuildClassDefinition(dataRecordType, out fullTypeName);
            CSharpCompilationOutput compilationOutput;
            if (!CSharpCompiler.TryCompileClass(classDefinition, out compilationOutput))
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

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : Vanrise.GenericData.Business.IDataRecordFiller
                    {                   
                        static #CLASSNAME#()
                        {
                             Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(#CLASSNAME#) #PROPERTIESTOSETSERIALIZED#);
                        }     
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
                    }
                }
                ");

            StringBuilder propertiesToSetSerializedBuilder = new StringBuilder();
            StringBuilder globalMembersBuilder = new StringBuilder();

            StringBuilder setFieldValueBuilder = new StringBuilder();
            StringBuilder getFieldValueBuilder = new StringBuilder();

            foreach (var field in dataRecordType.Fields)
            {
                globalMembersBuilder.AppendFormat("public {0} {1} {{ get; set; }}", CSharpCompiler.TypeToString(field.Type.GetRuntimeType()), field.Name);
                propertiesToSetSerializedBuilder.AppendFormat(", \"{0}\"", field.Name);
                setFieldValueBuilder.AppendFormat(@"case ""{0}"" : if(fieldValue != null) {0} = ({1})Convert.ChangeType(fieldValue, typeof({1})); break;", field.Name, field.Type.GetNonNullableRuntimeType());
                getFieldValueBuilder.AppendFormat(@"case ""{0}"" : return {0};", field.Name);
            }

            classDefinitionBuilder.Replace("#GLOBALMEMBERS#", globalMembersBuilder.ToString());
            classDefinitionBuilder.Replace("#PROPERTIESTOSETSERIALIZED#", propertiesToSetSerializedBuilder.ToString());

            classDefinitionBuilder.Replace("#SETFIELDMEMBERS#", setFieldValueBuilder.ToString());
            classDefinitionBuilder.Replace("#GETFIELDMEMBERS#", getFieldValueBuilder.ToString());

            string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.GenericData.Runtime");
            string className = "DataRecord";
            classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
            classDefinitionBuilder.Replace("#CLASSNAME#", className);
            fullTypeName = String.Format("{0}.{1}", classNamespace, className);

            return classDefinitionBuilder.ToString();
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
