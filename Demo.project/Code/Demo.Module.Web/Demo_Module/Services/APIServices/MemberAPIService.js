(function (appControllers) {
    "use strict";
    memberAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function memberAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Demo_Member";

        function GetFilteredMembers(input) {
            console.log("get member")
           var  a = BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredMembers"), input);
           console.log(a);
           return a;
        }
        function GetMemberById(memberId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetMemberById"),
                {
                    memberId: memberId
                });
        }

        function UpdateMember(member) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateMember"), member);
        }
        function AddMember(member) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddMember"), member);
        };

        return {
            GetFilteredMembers: GetFilteredMembers,
            GetMemberById: GetMemberById,
            UpdateMember: UpdateMember,
            AddMember: AddMember,
        };
    };
    appControllers.service("Demo_Module_MemberAPIService", memberAPIService);

})(appControllers);