﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class DataRecordFieldTypeConvertFlatFileValueContext : IDataRecordFieldTypeConvertFlatFileValueContext
    {
        public object FieldValue { get; set; }
    }
}
