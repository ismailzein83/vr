using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;
using System.Reflection;
using System.Reflection.Emit;
namespace Vanrise.GenericData.Business
{
    public class DataRecordTypeManager
    {
       
        #region Public Methods
        public IDataRetrievalResult<DataRecordTypeDetail> GetFilteredDataRecordTypes(DataRetrievalInput<DataRecordTypeQuery> input)
        {
            var allItems = GetCachedDataRecordTypes();

            Func<DataRecordType, bool> filterExpression = (itemObject) =>
                 (input.Query.Name == null || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, DataRecordTypeDetailMapper));
        }
        public DataRecordType GetDataRecordType(int dataRecordTypeId)
        {
            var dataRecordTypes = GetCachedDataRecordTypes();
            return dataRecordTypes.GetRecord(dataRecordTypeId);
        }
        public string GetDataRecordTypeName(int dataRecordTypeId)
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
            return dataRecordTypes.MapRecords(DataRecordTypeInfoMapper);
        }
        public Vanrise.Entities.InsertOperationOutput<DataRecordTypeDetail> AddDataRecordType(DataRecordType dataRecordType)
        {
            InsertOperationOutput<DataRecordTypeDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<DataRecordTypeDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int dataRecordTypeId = -1;


            IDataRecordTypeDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordTypeDataManager>();
            bool insertActionSucc = dataManager.AddDataRecordType(dataRecordType, out dataRecordTypeId);

            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired("GetDataRecordTypes");
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                dataRecordType.DataRecordTypeId = dataRecordTypeId;
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
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired("GetDataRecordTypes");
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = DataRecordTypeDetailMapper(dataRecordType);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public List<Vanrise.Entities.TemplateConfig> GetDataRecordFieldTypeTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.DataRecordFieldConfigType);
        }

        public Type GetDataRecordRuntimeType(int dataRecordTypeId)
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

        public dynamic CreateDataRecordObject(string dataRecordTypeName)
        {
            Type dataRecordRuntimeType = GetDataRecordRuntimeType(dataRecordTypeName);
            if (dataRecordRuntimeType != null)
                return Activator.CreateInstance(dataRecordRuntimeType);
            return null;
        }
    
        #endregion
      
        #region Private Methods
        private Dictionary<int, DataRecordType> GetCachedDataRecordTypes()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDataRecordTypes",
               () =>
               {
                   IDataRecordTypeDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordTypeDataManager>();
                   IEnumerable<DataRecordType> dataRecordTypes = dataManager.GetDataRecordTypes();
                   return dataRecordTypes.ToDictionary(kvp => kvp.DataRecordTypeId, kvp => kvp);
               });
        }
        
        private Type GetOrCreateRuntimeType(DataRecordType dataRecordType)
        {
            string typeSignature = String.Format("DRT_{0}_{1:yyyyMMdd_HHmmss}", dataRecordType.DataRecordTypeId, DateTime.Now);
            var assemblyName = new AssemblyName(typeSignature);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeSignature, TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , null);

            ConstructorBuilder constructor = typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            if (dataRecordType.Fields != null)
            {
                foreach (var field in dataRecordType.Fields)
                {
                    CreateProperty(typeBuilder, field.Name, field.Type.GetRuntimeType());
                }
            }

            return typeBuilder.CreateType();
        }

        private void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr =
                tb.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new[] { propertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }

        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDataRecordTypeDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
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
             dataRecordTypeDetail.ParentName = GetDataRecordTypeName((int)dataRecordTypeObject.ParentId);
            return dataRecordTypeDetail;
        }
        private DataRecordTypeInfo DataRecordTypeInfoMapper(DataRecordType dataRecordTypeObject)
        {
            DataRecordTypeInfo dataRecordTypeInfo = new DataRecordTypeInfo();
            dataRecordTypeInfo.DataRecordTypeId = dataRecordTypeObject.DataRecordTypeId;
            dataRecordTypeInfo.Name =  dataRecordTypeObject.Name;
            return dataRecordTypeInfo;
        }
        #endregion
    }
}
