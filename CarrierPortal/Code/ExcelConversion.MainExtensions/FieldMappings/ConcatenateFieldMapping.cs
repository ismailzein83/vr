﻿using ExcelConversion.Business;
using ExcelConversion.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConversion.MainExtensions.FieldMappings
{
    public class ConcatenateFieldMapping : FieldMapping
    {
        public List<ConcatenatedPart> Parts { get; set; }

        public override object GetFieldValue(IGetFieldValueContext context)
        {
            if (this.Parts == null)
                throw new NullReferenceException("Parts");
            StringBuilder builder = new StringBuilder();
            GetConcatenatedPartTextContext getPartTextContext = new GetConcatenatedPartTextContext
            {
                Workbook = context.Workbook,
                Sheet = context.Sheet,
                Row = context.Row
            };

            foreach (var part in this.Parts)
            {
                builder.Append(part.GetPartText(getPartTextContext));
            }
            return builder.ToString();
        }
    }
}
