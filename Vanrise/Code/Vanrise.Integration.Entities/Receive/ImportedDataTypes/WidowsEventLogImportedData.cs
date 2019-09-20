using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class WidowsEventLogImportedData : IImportedData
    {
        public List<WindowsEventLog> Events { get; set; }
        public string Description
        {
            get
            {
                StringBuilder descriptionBuilder = new StringBuilder();
                if (Events != null)
                    descriptionBuilder.AppendFormat("Number Of Events'{0}'", Events.Count);
                return descriptionBuilder.ToString();
            }
        }
        public long? BatchSize { get { return null; } }

        public BatchState BatchState { get; set; }

        public bool? IsDuplicateSameSize { get; set; }

        public bool IsEmpty { get; set; }

        public bool IsFile { get { return false; } }

        public bool IsMultipleReadings { get { return false; } }

        public void OnDisposed()
        {
          
        }
    }
    public class WindowsEventLog
    {
        public string Description { get; set; }
        public DateTime? TimeCreated { get; set; }
        public string MachineName { get; set; }
        public string LevelDisplayName { get; set; }
        public string TaskDisplayName { get; set; }
        public string ProviderName { get; set; }
        public string DescriptionXml { get; set; }

    }
}
