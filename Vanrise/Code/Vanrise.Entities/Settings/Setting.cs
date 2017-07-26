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
        public virtual bool IsValid()
        {
            return true;
        }
    }

    public class SettingToEdit
    {
        public Guid SettingId { get; set; }

        public string Name { get; set; }

        public SettingData Data { get; set; }
    }
}