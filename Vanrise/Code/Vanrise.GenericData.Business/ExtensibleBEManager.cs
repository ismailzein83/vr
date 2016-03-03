﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public abstract class ExtensibleBEManager : BaseBEManager,IExtensibleBEManager
    {
        protected abstract string _businessEntityName{get;}

        public int GetBusinessEntityDefinitionId()
        {
            BusinessEntityDefinitionManager manager = new BusinessEntityDefinitionManager();
            return manager.GetBusinessEntityDefinitionId(this._businessEntityName);
        }
        public ExtensibleBEItemRuntime GetExtensibleBEItemRuntime(int dataRecordTypeId, int businessEntityId)
        {
            GenericEditorManager manager = new GenericEditorManager();
            return manager.GetExtensibleBEItemRuntime(businessEntityId, dataRecordTypeId);
        }
        public IEnumerable<DataRecordTypeInfo> GetDataRecordTypesInfo(int businessEntityId)
        {
            GenericEditorManager manager = new GenericEditorManager();
            return manager.GetDataRecordTypesInfo(businessEntityId);
        }
    }
}
