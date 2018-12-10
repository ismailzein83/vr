using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.RA.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class SpecialNumbersManager
    {
        //static Guid beDefinitionId = new Guid("2CD325A0-6DD1-4F89-9152-81CC48C77ECE");
        //public static long GetSpecialNumberGroupId(string specialNumber)
        //{
        //    int number = 0;
        //    Int32.TryParse(specialNumber, out number);
        //    var specialNumberGroupsByNumber = GetCachedSpecialNumberGroupsByNumber();
        //    return specialNumberGroupsByNumber.GetRecord(number);
        //}
        //private static  Dictionary<long, SpecialNumberGroup> GetCachedSpecialNumberGroups()
        //{
        //    Dictionary<long, SpecialNumberGroup> specialNumberGroupsByGroupId = new Dictionary<long, SpecialNumberGroup>();
        //    IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
        //    return genericBusinessEntityManager.GetCachedOrCreate("GetCachedSpecialNumberGroups", beDefinitionId, () =>
        //    {
        //        List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(beDefinitionId);
        //        if (genericBusinessEntities != null)
        //        {
        //            foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
        //            {
        //                if (genericBusinessEntity.FieldValues == null)
        //                    continue;

        //                SpecialNumberGroup specialNumberGroup = new SpecialNumberGroup
        //                {
        //                    ID = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
        //                    GroupName = (string)genericBusinessEntity.FieldValues.GetRecord("GroupName"),
        //                    Settings = Serializer.Deserialize<SpecialNumbersSetting>(Serializer.Serialize(genericBusinessEntity.FieldValues.GetRecord("Settings")))
        //                };
        //                specialNumberGroupsByGroupId.Add(specialNumberGroup.ID, specialNumberGroup);
        //            }
        //        }

        //        return specialNumberGroupsByGroupId;
        //    });
        //}
        //private static Dictionary<int, long> GetCachedSpecialNumberGroupsByNumber()
        //{
        //    Dictionary<int, long> specialNumberGroupsByNumber = new Dictionary<int, long>();
        //    IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
        //    return genericBusinessEntityManager.GetCachedOrCreate("GetCachedSpecialNumberGroupsByNumber", beDefinitionId, () =>
        //     {
        //         var specialNumberGroupsByGroupId = GetCachedSpecialNumberGroups();
        //         if (specialNumberGroupsByGroupId != null)
        //         {
        //             foreach (var specialNumberGroup in specialNumberGroupsByGroupId)
        //             {
        //                 specialNumberGroup.Value.ThrowIfNull("specialNumberGroup");
        //                 specialNumberGroup.Value.Settings.ThrowIfNull("Settings");
        //                 specialNumberGroup.Value.Settings.Numbers.ThrowIfNull("SpecialNumbers");
        //                 specialNumberGroup.Value.Settings.Range.ThrowIfNull("SpecialNumbers");
        //                 foreach (var specialNumber in specialNumberGroup.Value.Settings.Numbers)
        //                 {
        //                     if (specialNumberGroupsByNumber.ContainsKey(specialNumber))
        //                         throw new VRBusinessException(string.Format("Key '{0}' already exists", specialNumber));

        //                     specialNumberGroupsByNumber.Add(specialNumber, specialNumberGroup.Key);
        //                 }
        //                 foreach (var range in specialNumberGroup.Value.Settings.Range)
        //                 {
        //                     for (var specialNumberFromRange = range.From; specialNumberFromRange <= range.To; specialNumberFromRange++)
        //                     {
        //                         if (specialNumberGroupsByNumber.ContainsKey(specialNumberFromRange))
        //                             throw new VRBusinessException(string.Format("Key '{0}' already exists", specialNumberFromRange));

        //                         specialNumberGroupsByNumber.Add(specialNumberFromRange, specialNumberGroup.Key);
        //                     }
        //                 }

        //             }
        //         }

        //         return specialNumberGroupsByNumber;
        //     });
        //}
    }
}


