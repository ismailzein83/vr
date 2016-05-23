namespace Vanrise.Entities
{
    public class Setting
    {
        public int SettingId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Category { get; set; }

        public SettingConfiguration Settings { get; set; }

        public object Data { get; set; }
    }

    public class SettingConfiguration
    {
        public string Editor { get; set; }
    }

    public abstract class SettingData 
    { 

    }
}