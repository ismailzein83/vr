using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DatabaseStructure
{
    public abstract class VRDBStructureConvertor
    {
        public abstract VRDBType DBType { get; }

        public abstract string GenerateConvertedScript(IVRDBStructureConvertorGenerateConvertedScriptContext context);
    }

    public interface IVRDBStructureConvertorGenerateConvertedScriptContext
    {
        VRDBStructure DBStructure { get; }
    }
}
