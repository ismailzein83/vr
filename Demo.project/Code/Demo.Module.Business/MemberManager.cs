using Demo.Module.Data;
using Demo.Module.Entities.Member;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Demo.Module.Business
{
   public  class MemberManager
    {
        FamilyManager _familyManager = new FamilyManager();
        #region Public Methods
        public IEnumerable<MemberShapeConfig> GetMemberShapeConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<MemberShapeConfig>(MemberShapeConfig.EXTENSION_TYPE);
        }
        public IDataRetrievalResult<MemberDetails> GetFilteredMembers(DataRetrievalInput<MemberQuery> input)
        {
            var allMembers = GetCachedMembers();
            Func<Member, bool> filterExpression = (member) =>
            {
                if (input.Query.Name != null && !member.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (input.Query.FamilyIds != null && !input.Query.FamilyIds.Contains(member.FamilyId))
                    return false;
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allMembers.ToBigResult(input, filterExpression, MemberDetailMapper));

        }


        public InsertOperationOutput<MemberDetails> AddMember(Member member)
        {
            IMemberDataManager memberDataManager = DemoModuleFactory.GetDataManager<IMemberDataManager>();
            InsertOperationOutput<MemberDetails> insertOperationOutput = new InsertOperationOutput<MemberDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long memberId = -1;

            bool insertActionSuccess = memberDataManager.Insert(member, out memberId);
            if (insertActionSuccess)
            {
                member.MemberId = memberId;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = MemberDetailMapper(member);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
        public Member GetMemberById(long memberId)
        {
            var allMembers = GetCachedMembers();
            return allMembers.GetRecord(memberId);
        }

        public UpdateOperationOutput<MemberDetails> UpdateMember(Member member)
        {
            IMemberDataManager memberDataManager = DemoModuleFactory.GetDataManager<IMemberDataManager>();
            UpdateOperationOutput<MemberDetails> updateOperationOutput = new UpdateOperationOutput<MemberDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = memberDataManager.Update(member);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = MemberDetailMapper(member);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IMemberDataManager memberDataManager = DemoModuleFactory.GetDataManager<IMemberDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return memberDataManager.AreMembersUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<long, Member> GetCachedMembers()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedMembers", () =>
               {
                   IMemberDataManager memberDataManager = DemoModuleFactory.GetDataManager<IMemberDataManager>();
                   List<Member> members = memberDataManager.GetMembers();
                   return members.ToDictionary(member => member.MemberId, member => member);
               });
        }
        #endregion

        #region Mappers
        public MemberDetails MemberDetailMapper(Member member)
        {
            return new MemberDetails
            {
                Name = member.Name,
                MemberId = member.MemberId,
                FamilyName = _familyManager.GetFamilyName(member.FamilyId)
            };
        }
        #endregion

    }
}
