﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Data;
using Vanrise.Caching;

namespace Vanrise.GenericData.Business
{
    public class GenericEditorManager
    {
        public GenericEditor GetEditor(int editorId)
        {
            return null;
        }
        public GenericEditor GetEditor(int businessEntityId, int dataRecordTypeId)
        {
            var cachedGenericEditorDefinitions = GetCachedGenericEditorDefinitions();
            var genericEditor = cachedGenericEditorDefinitions.FindRecord(x => x.BusinessEntityId == businessEntityId && x.Details.DataRecordTypeId == dataRecordTypeId);
            if (genericEditor != null)
                return genericEditor.Details;
            return null;
        }
        public GenericEditorRuntime GetEditorRuntime(int businessEntityId, int dataRecordTypeId)
        {
            var genericEditor = GetEditor(businessEntityId, dataRecordTypeId);
            if (genericEditor == null)
                return null;

            GenericEditorRuntime editorRuntime = new GenericEditorRuntime();
            BuildEditorRuntime(genericEditor, editorRuntime);
            return editorRuntime;
        }
        public GenericEditorDefinition GetGenericEditorDefinition(int editorId)
        {
            var cachedGenericEditorDefinitions = GetCachedGenericEditorDefinitions();
            return cachedGenericEditorDefinitions.GetRecord(editorId);

        }
        public GenericEditorRuntime GetEditorRuntime(int editorId)
        {
            var genericEditor = GetEditor(editorId);
            if (genericEditor == null)
                return null;

            GenericEditorRuntime editorRuntime = new GenericEditorRuntime();
            BuildEditorRuntime(genericEditor, editorRuntime);
            return editorRuntime;
        }
        public GenericEditorRuntime GetEditorRuntimeMock(int editorId)
        {
            var genericEditor = new GenericEditor();
            if (editorId == 1)
            {
                genericEditor.DataRecordTypeId = 1;
                int i = 0;
                genericEditor.Sections = new List<GenericEditorSection>();

                genericEditor.Sections.Add(new GenericEditorSection
                {
                    Rows = new List<GenericEditorRow>()
                });
                foreach (GenericEditorSection section in genericEditor.Sections)
                {
                    section.SectionTitle = "Contacts" + i++;
                    section.Rows.Add(new GenericEditorRow
                    {
                        Fields = new List<GenericEditorField>()
                    });
                    section.Rows.Add(new GenericEditorRow
                    {
                        Fields = new List<GenericEditorField>()
                    });

                    section.Rows[0].Fields.Add(new GenericEditorField
                    {
                        FieldTitle = "Email" + i++,
                        FieldPath = "Email"
                    });
                    section.Rows[0].Fields.Add(new GenericEditorField
                    {
                        FieldTitle = "Mobile" + i++,
                        FieldPath = "Mobile"
                    });
                    section.Rows[1].Fields.Add(new GenericEditorField
                    {
                        FieldTitle = "Address" + i++,
                        FieldPath = "Address"
                    });
                    section.Rows[1].Fields.Add(new GenericEditorField
                    {
                        FieldTitle = "Phone" + i++,
                        FieldPath = "Phone"
                    });
                }
            }
            else
            {
                genericEditor.DataRecordTypeId = 1;
                int i = 0;
                genericEditor.Sections = new List<GenericEditorSection>();

                genericEditor.Sections.Add(new GenericEditorSection
                {
                    Rows = new List<GenericEditorRow>()
                });
              
                foreach (GenericEditorSection section in genericEditor.Sections)
                {
                    section.SectionTitle = "Contacts" + i++;
                    section.Rows.Add(new GenericEditorRow
                    {
                        Fields = new List<GenericEditorField>()
                    });
                    
                    foreach (GenericEditorRow field in section.Rows)
                    {
                        field.Fields.Add(new GenericEditorField
                        {
                            FieldTitle = "Email" + i++,
                            FieldPath = "Email"
                        });
                    }
                }
            }
            

            GenericEditorRuntime editorRuntime = new GenericEditorRuntime();
            BuildEditorRuntime(genericEditor, editorRuntime);
            return editorRuntime;
        }

        public List<DataRecordTypeInfo> GetDataRecordTypesInfo(int businessEntityId)
        {
             var cachedGenericEditorDefinitions = GetCachedGenericEditorDefinitions();
             var genericEditors = cachedGenericEditorDefinitions.FindAllRecords(x=>x.BusinessEntityId == businessEntityId);
             var dataRecordTypeManager = new DataRecordTypeManager();
             List<DataRecordTypeInfo> recordTypesInfo = new List<DataRecordTypeInfo>();
             foreach(GenericEditorDefinition editor in genericEditors)
             {
                var recordTypeInfo =  dataRecordTypeManager.GetDataRecordTypeInfo(editor.Details.DataRecordTypeId);
                if(recordTypeInfo !=null)
                    recordTypesInfo.Add(recordTypeInfo);
             }
             return recordTypesInfo;
        }
        public Vanrise.Entities.InsertOperationOutput<GenericEditorDefinitionDetail> AddGenericEditor(GenericEditorDefinition genericEditorDefinition)
        {
            InsertOperationOutput<GenericEditorDefinitionDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<GenericEditorDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int genericEditorDefinitionId = -1;

            IGenericEditorDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericEditorDataManager>();
            bool insertActionSucc = dataManager.AddGenericEditorDefinition(genericEditorDefinition, out genericEditorDefinitionId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                genericEditorDefinition.GenericEditorDefinitionId = genericEditorDefinitionId;
                insertOperationOutput.InsertedObject = GenericEditorDefinitionDetailMapper(genericEditorDefinition);

                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<GenericEditorDefinitionDetail> UpdateGenericEditor(GenericEditorDefinition genericEditorDefinition)
        {
            UpdateOperationOutput<GenericEditorDefinitionDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<GenericEditorDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IGenericEditorDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericEditorDataManager>();
            bool updateActionSucc = dataManager.UpdateGenericEditorDefinition(genericEditorDefinition);

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.UpdatedObject = GenericEditorDefinitionDetailMapper(genericEditorDefinition);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public Vanrise.Entities.IDataRetrievalResult<GenericEditorDefinitionDetail> GetFilteredGenericEditorDefinitions(Vanrise.Entities.DataRetrievalInput<GenericEditorDefinitionQuery> input)
        {
            var cachedGenericEditorDefinitions = GetCachedGenericEditorDefinitions();

            Func<GenericEditorDefinition, bool> filterExpression = (genericEditor) => (input.Query.BusinessEntityDefinitionId == genericEditor.BusinessEntityId);
            return DataRetrievalManager.Instance.ProcessResult(input, cachedGenericEditorDefinitions.ToBigResult(input, filterExpression, GenericEditorDefinitionDetailMapper));
        }

        #region Private Methods

        private Dictionary<int, GenericEditorDefinition> GetCachedGenericEditorDefinitions()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetGenericEditorDefinitions",
               () =>
               {
                   IGenericEditorDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericEditorDataManager>();
                   IEnumerable<GenericEditorDefinition> genericEditorDefinitions = dataManager.GetGenericEditorDefinitions();
                   return genericEditorDefinitions.ToDictionary(kvp => kvp.GenericEditorDefinitionId, kvp => kvp);
               });
        }

        private void BuildEditorRuntime(GenericEditor genericEditor, GenericEditorRuntime editorRuntime)
        {
            BuildEditorRuntimeSections(genericEditor, editorRuntime);
        }

        private void BuildEditorRuntimeSections(GenericEditor genericEditor, GenericEditorRuntime editorRuntime)
        {
            if (genericEditor.Sections != null)
            {
                editorRuntime.Sections = new List<GenericEditorRuntimeSection>();
                foreach (var section in genericEditor.Sections)
                {
                    var runtimeSection = new GenericEditorRuntimeSection();
                    runtimeSection.SectionTitle = section.SectionTitle;
                    editorRuntime.Sections.Add(runtimeSection);
                    BuildEditorRuntimeRows(genericEditor, section, runtimeSection);
                }
            }
        }

        private void BuildEditorRuntimeRows(GenericEditor genericEditor, GenericEditorSection section, GenericEditorRuntimeSection runtimeSection)
        {
            if (section.Rows != null)
            {
                var dataRecordTypeManager = new DataRecordTypeManager();
                var fields = dataRecordTypeManager.GetDataRecordTypeFields(genericEditor.DataRecordTypeId);
                if (fields == null)
                    throw new NullReferenceException(String.Format("fields of DataRecordType '{0}'", genericEditor.DataRecordTypeId));
                runtimeSection.Rows = new List<GenericEditorRuntimeRow>();
                foreach (var row in section.Rows)
                {
                    var runtimeRow = new GenericEditorRuntimeRow();
                    runtimeSection.Rows.Add(runtimeRow);
                    BuildEditorRuntimeFields(genericEditor, row, runtimeRow, fields);
                }
            }
        }

        private void BuildEditorRuntimeFields(GenericEditor genericEditor, GenericEditorRow row, GenericEditorRuntimeRow runtimeRow, List<DataRecordField> dataRecordTypeFields)
        {
            if (row.Fields != null)
            {
                runtimeRow.Fields = new List<GenericEditorRuntimeField>();
                foreach (var field in row.Fields)
                {
                    var runtimeField = new GenericEditorRuntimeField();
                    runtimeRow.Fields.Add(runtimeField);
                    runtimeField.FieldTitle = field.FieldTitle;
                    runtimeField.FieldPath = field.FieldPath;
                    var dataRecordTypeField = dataRecordTypeFields.FindRecord(itm => itm.Name == runtimeField.FieldPath);
                    if (dataRecordTypeField == null)
                        throw new NullReferenceException(String.Format("DataRecordType '{0}' dataRecordTypeField '{1}'", genericEditor.DataRecordTypeId, runtimeField.FieldPath));
                    runtimeField.FieldType = dataRecordTypeField.Type;
                }
            }
        }

        #endregion


        #region Private Classes
        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IGenericEditorDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericEditorDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return this.IsCacheExpired();
            }

            public bool IsCacheExpired()
            {
                return _dataManager.AreGenericEditorDefinitionUpdated(ref _updateHandle);
            }
        }


        #endregion

        #region Mappers

        private GenericEditorDefinitionDetail GenericEditorDefinitionDetailMapper(GenericEditorDefinition genericEditorDefinition)
        {
            GenericEditorDefinitionDetail genericEditorDefinitionDetail = new GenericEditorDefinitionDetail();
            genericEditorDefinitionDetail.Entity = genericEditorDefinition;

            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            if (genericEditorDefinition.Details != null)
            {
               genericEditorDefinitionDetail.RecordTypeName= dataRecordTypeManager.GetDataRecordTypeName(genericEditorDefinition.Details.DataRecordTypeId);

            }
            return genericEditorDefinitionDetail;
        }

        #endregion
    }
}