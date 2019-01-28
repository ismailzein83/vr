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
    public class SwitchCodeManager
    {
        public static Guid _definitionId = new Guid("05E5BBB0-60D3-4D82-B25C-8EA378AFDF84");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        public IEnumerable<SwitchCode> GetAllSwitchCodes()
        {
            return GetCachedSwitchCodes().Values;
        }

        public IEnumerable<SwitchCodeDetail> GetSwitchCodesInfo(SwitchCodeInfoFilter filter)
        {
            var switchCodes = GetCachedSwitchCodes();
            Func<SwitchCode, bool> filterFunc = (switchCode) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null && filter.Filters.Count() > 0)
                    {
                        var context = new SwitchCodeFilterContext
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
        private Dictionary<Guid, SwitchCode> GetCachedSwitchCodes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedSwitchCodes", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, SwitchCode> result = new Dictionary<Guid, SwitchCode>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        SwitchCode switchCode = new SwitchCode()
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
        private SwitchCodeDetail SwitchCodeInfoMapper(SwitchCode switchCode)
        {
            return new SwitchCodeDetail
            {
                ID = switchCode.ID,
                SwitchId = switchCode.SwitchId
            };
        }

    }
   
}
