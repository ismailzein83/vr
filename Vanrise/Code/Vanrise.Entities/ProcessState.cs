namespace Vanrise.Entities
{
    public class ProcessState
    {
        public string UniqueName { get; set; }
        public ProcessStateSettings Settings { get; set; }
    }

    public abstract class ProcessStateSettings
    {

    }
}