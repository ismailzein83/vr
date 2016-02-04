using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Normalization
{
    public class NormalizationRule : GenericRule
    {
        public Vanrise.Rules.Normalization.NormalizeNumberSettings Settings { get; set; }
    }
}
