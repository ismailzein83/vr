using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public enum EngineType { Fast = 0, Regex = 1 }
    public enum PrefixSign { Plus = 0, Minus = 1 }
    public enum CLIType { FixedCLI = 0 , PoolBasedCLI = 1 }
    public class TranslationRule
    {
        public int TranslationRuleId { get; set; }
        public String Name { get; set; }
        public EngineType EngineType { get; set; }
        public PrefixSign? DNISPatternSign { get; set; }
        public String DNISPattern { get; set; }
        public CLIType CLIType { get; set; }
        public FixedCLISettings FixedCLISettings { get; set; }
        public PoolBasedCLISettings PoolBasedCLISettings { get; set; }
        public DateTime CreationDate { get; set; }
    }

    public class FixedCLISettings
    {
        public PrefixSign? CLIPatternSign { get; set; }
        public String CLIPattern { get; set; }
    }
    public class PoolBasedCLISettings
    {
        public List<string> CLIPatterns { get; set; }
        public string Prefix { get; set; }
        public string Destination { get; set; }
        public int RandMin { get; set; }
        public int RandMax { get; set; }
        public string DisplayName { get; set; }
    }
    public class TranslationRuleToAdd
    {

    }
    public class TranslationRuleToUpdate
    {

    }
}
