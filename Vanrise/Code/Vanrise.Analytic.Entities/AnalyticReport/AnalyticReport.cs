﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public enum AccessType { Public = 0, Private = 1 }
    public class AnalyticReport
    {
        public Guid AnalyticReportId { get; set; }
        public Guid? DevProjectId { get; set; }
        public int UserID { get; set; }
        public AccessType AccessType { get; set; }
        public string Name { get; set; }

        string _title;
        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(_title))
                    _title = Name;
                return _title;
            }
            set { _title = value; }
        }

        public AnalyticReportSettings Settings { get; set; }
    }
}
