﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public abstract class VRGenericEditorDefinitionSetting
    {
        public abstract Guid ConfigId { get; }
        public virtual string RuntimeEditor { get; set; }
        public virtual List<GridColumnAttribute> GetGridColumnsAttributes(IGetGenericEditorColumnsInfoContext context)
        {
            return null;
        }
        public virtual void TryTranslate() { }
	
    }
    public interface IGetGenericEditorColumnsInfoContext
    {
        Guid DataRecordTypeId { get; }
    }
    public class GetGenericEditorColumnsInfoContext : IGetGenericEditorColumnsInfoContext
    {
        public Guid DataRecordTypeId { get; set; }
    }

}
