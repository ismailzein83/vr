using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class SalePriceListTemplateSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract byte[] Execute(ISalePriceListTemplateSettingsContext context);

        public virtual void OnBeforeSave(IPriceListTemplateOnBeforeSaveContext context)
        {
        }

        public virtual void OnAfterSave(IPriceListTemplateOnAfterSaveContext context)
        {
        }
    }

    public interface IPriceListTemplateOnBeforeSaveContext
    {
        SaveOperationType SaveOperationType { get; }
        int? TemplateId { get; }
    }

    public interface IPriceListTemplateOnAfterSaveContext
    {
        SaveOperationType SaveOperationType { get; }
        int TemplateId { get; }
    }

}
