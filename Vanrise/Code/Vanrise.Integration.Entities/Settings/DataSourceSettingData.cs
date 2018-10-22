using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Integration.Entities
{
    public class DataSourceSettingData : SettingData
    {
        public FileDataSourceSettings FileDataSourceSettings { get; set; }
    }

    public class FileDataSourceSettings
    {
        public List<PeakDateTimeRange> PeakDateTimeRanges { get; set; }

        public List<FileDataSourceDefinition> FileDataSourceDefinitions { get; set; }
    }

    public class PeakDateTimeRange
    {
        public Guid PeakDateTimeRangeId { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }
    }
}