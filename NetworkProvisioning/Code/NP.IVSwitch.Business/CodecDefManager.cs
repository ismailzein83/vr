using NP.IVSwitch.Data;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;

namespace NP.IVSwitch.Business
{
    public class CodecDefManager
    {
        #region Public Methods        

       public IEnumerable<CodecDefInfo> GetCodecDefsInfo(CodecDefFilter filter)
        {
            Func<CodecDef, bool> filterExpression = null;

            return this.GetCachedCodecDefs().MapRecords(CodecDefInfoMapper, filterExpression).OrderBy(x => x.Description);
        }

        public List<CodecDef> GetCodecDefList(List<int> CodecIdList)
        {
            Func<CodecDef, bool> filterExpression = (x) => (CodecIdList == null ||  CodecIdList.Contains(x.CodecId)); ;

            return this.GetCachedCodecDefs().FindAllRecords(filterExpression).OrderBy(x => x.Description).ToList();

        }

        public Dictionary<int, CodecDef> GetAll()
        {
            Dictionary<int, CodecDef> cachedCodecDef = this.GetCachedCodecDefs();
            return cachedCodecDef;
        }

        #endregion
        
 
        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICodecDefDataManager _dataManager = IVSwitchDataManagerFactory.GetDataManager<ICodecDefDataManager>();
            protected override bool IsTimeExpirable { get { return true; } }

        }
        #endregion

        #region Private Methods

        Dictionary<int, CodecDef> GetCachedCodecDefs()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCodecDef",
                () =>
                {
                    ICodecDefDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<ICodecDefDataManager>();
                    Helper.SetSwitchConfig(dataManager);
                    return dataManager.GetCodecDefs().ToDictionary(x => x.CodecId, x => x);
                });
        }

        #endregion



        #region Mappers

       

        public CodecDefInfo CodecDefInfoMapper(CodecDef codecDef)
        {
            CodecDefInfo codecDefInfo = new CodecDefInfo()
            {
                CodecId = codecDef.CodecId,
                Description = codecDef.Description,
                FsName = codecDef.FsName,
                ClockRate = codecDef.ClockRate,
                PassThru = codecDef.PassThru,
                DefaultMsPerPacket = codecDef.DefaultMsPerPacket

            };
            return codecDefInfo;
        }

        #endregion
    }
}
