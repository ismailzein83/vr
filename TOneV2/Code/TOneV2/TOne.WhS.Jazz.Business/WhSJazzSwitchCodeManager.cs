using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using TOne.WhS.Jazz.Entities;
namespace TOne.WhS.Jazz.Business
{
    public class WhSJazzSwitchCodeManager
    {
        public static Guid _definitionId = new Guid("05E5BBB0-60D3-4D82-B25C-8EA378AFDF84");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        public IEnumerable<WhSJazzSwitchCode> GetAllSwitchCodes()
        {
            return GetCachedSwitchCodes().Values;
        }

        public IEnumerable<WhSJazzSwitchCodeDetail> GetSwitchCodesInfo(WhSJazzSwitchCodeInfoFilter filter)
        {
            var switchCodes = GetCachedSwitchCodes();
            Func<WhSJazzSwitchCode, bool> filterFunc = (switchCode) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null && filter.Filters.Count() > 0)
                    {
                        var context = new WhSJazzSwitchCodeFilterContext
                        {
                            SwitchCode = switchCode
                        };
                        foreach (var switchCodeFilter in filter.Filters)
                        {
                            if (!switchCodeFilter.IsMatch(context))
                                return false;
                        }
                    }
                }
                return true;
            };
            return switchCodes.MapRecords((record) =>
            {
                return SwitchCodeInfoMapper(record);
            }, filterFunc);

        }
        private Dictionary<Guid, WhSJazzSwitchCode> GetCachedSwitchCodes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedSwitchCodes", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, WhSJazzSwitchCode> result = new Dictionary<Guid, WhSJazzSwitchCode>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        WhSJazzSwitchCode switchCode = new WhSJazzSwitchCode()
                        {
                            ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            SwitchId = (int)genericBusinessEntity.FieldValues.GetRecord("SwitchId"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")

                        };
                        result.Add(switchCode.ID, switchCode);
                    }
                }

                return result;
            });
        }
        private WhSJazzSwitchCodeDetail SwitchCodeInfoMapper(WhSJazzSwitchCode switchCode)
        {
            return new WhSJazzSwitchCodeDetail
            {
                ID = switchCode.ID,
                SwitchId = switchCode.SwitchId
            };
        }

    }
   
}
