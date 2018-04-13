using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class CreateGenericRuleFromExcelContext : ICreateGenericRuleFromExcelContext
    {
        public GenericRule GenericRule { get; set; }

        public string ErrorMessage { get; set; }

        public Dictionary<string, Object> ParsedGenericRulesFields { get; set; }
    }

}
