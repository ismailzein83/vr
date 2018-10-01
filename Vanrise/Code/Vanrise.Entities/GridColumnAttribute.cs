﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public enum GridColCSSClassValue { Green = 0, Yellow = 1, Blue = 2, Red = 3 }
    public class GridColumnAttribute
    {
        public string Type { get; set; }
        public string HeaderText { get; set; }
        public Object Field { get; set; }
        public bool? IsFieldDynamic { get; set; }
        public int? WidthFactor { get; set; }
        public int? FixedWidth { get; set; }
        public bool? IsClickable { get; set; }
        public string OnClicked { get; set; }
        public string Tag { get; set; }
        public bool? DisableSorting { get; set; }
        public string NumberPrecision { get; set; }
        public string HeaderDescription { get; set; }
        public GridColCSSClassValue? GridColCSSClassValue { get; set; }
    }
}
