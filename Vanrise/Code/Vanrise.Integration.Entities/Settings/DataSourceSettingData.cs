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
        public List<PeakTimeRange> PeakTimeRanges { get; set; }

        public List<FileDataSourceDefinition> FileDataSourceDefinitions { get; set; }
    }

    public class PeakTimeRange
    {
        public Guid PeakTimeRangeId { get; set; }

        public Time From { get; set; }

        public Time To { get; set; }
    }
}