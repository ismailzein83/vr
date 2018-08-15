(function (appControllers) {

    "use strict";
    Comment.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function Comment(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = 'VRComment';

        function GetFilteredVRComments(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFilteredVRComments"), input);
        }
        function AddVRComment(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "AddVRComment"), input);
        }

        
        return ({
            GetFilteredVRComments: GetFilteredVRComments,
            AddVRComment: AddVRComment
           
        });
    }

    appControllers.service('VRCommon_VRCommentAPIService', Comment);

})(appControllers);