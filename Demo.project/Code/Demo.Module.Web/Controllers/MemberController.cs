using Demo.Module.Business;
using Demo.Module.Entities;
using Demo.Module.Entities.Member;
using Demo.Module.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Demo_Member")]
    [JSONWithTypeAttribute]
    public class MemberController:BaseAPIController
    {
        MemberManager memberManager = new MemberManager();
        [HttpPost]
        [Route("GetFilteredMembers")]
        public object GetFilteredMembers(DataRetrievalInput<MemberQuery> input)
        {
            return GetWebResponse(input, memberManager.GetFilteredMembers(input));
        }

        [HttpGet]
        [Route("GetMemberById")]
        public Member GetMemberById(long memberId)
        {
            return memberManager.GetMemberById(memberId);
        }

        [HttpPost]
        [Route("UpdateMember")]
        public UpdateOperationOutput<MemberDetails> UpdateMember(Member member)
        {
            return memberManager.UpdateMember(member);
        }

        [HttpPost]
        [Route("AddMember")]
        public InsertOperationOutput<MemberDetails> AddMember(Member member)
        {
            return memberManager.AddMember(member);
        }
        [HttpGet]
        [Route("GetMemberShapeConfigs")]
        public IEnumerable<MemberShapeConfig> GetChildShapeConfigs()
        {
            return memberManager.GetMemberShapeConfigs();
        }
    }
}