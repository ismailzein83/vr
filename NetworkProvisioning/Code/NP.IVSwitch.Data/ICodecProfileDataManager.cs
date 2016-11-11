using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Data
{
    public interface ICodecProfileDataManager : IDataManager
    {
        List<CodecProfile> GetCodecProfiles();
        bool Update(CodecProfile codecProfile, Dictionary<int, CodecDef> cachedCodecDef);
        bool Insert(CodecProfile codecProfile, Dictionary<int, CodecDef> cachedCodecDef, out int insertedId);

    }
}
