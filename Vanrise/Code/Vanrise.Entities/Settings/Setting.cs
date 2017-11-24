using System;
namespace Vanrise.Entities
{
    public class Setting
    {
        public Guid SettingId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Category { get; set; }

        public SettingConfiguration Settings { get; set; }

        public SettingData Data { get; set; }

        public bool IsTechnical { get; set; }
    }

    public class SettingConfiguration
    {
        public string Editor { get; set; }
    }

    public abstract class SettingData
    {
        public virtual bool IsValid(ISettingDataValidationContext context)
        {
            return true;
        }
        public virtual void OnBeforeSave(ISettingOnBeforeSaveContext context)
        {
        }
        public virtual void OnAfterSave(ISettingOnAfterSaveContext context)
        {
        }
    }
    public interface ISettingDataValidationContext
    {
        string ErrorMessage { get; set; }
    }
    public class SettingDataValidationContext : ISettingDataValidationContext
    {
        public string ErrorMessage { get; set; }
    }
    public class SettingToEdit
    {
        public Guid SettingId { get; set; }

        public string Name { get; set; }

        public SettingData Data { get; set; }
    }

    public interface ISettingOnBeforeSaveContext
    {
        Guid SettingId { get; }
        SaveOperationType SaveOperationType { get; }
        SettingData CurrentSettingData { get; }
        SettingData NewSettingData { get; }
    }

    public interface ISettingOnAfterSaveContext
    {

    }

}