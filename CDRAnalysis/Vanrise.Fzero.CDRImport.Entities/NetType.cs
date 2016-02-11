using System.ComponentModel;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public enum NetType
    {
        [Description("Others")]
        Others = 0,

        [Description("Local")]
        Local = 1,

        [Description("International")]
        International = 2
    };

}
