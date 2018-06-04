(function (appControllers) {
    "use strict";
   familyAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function familyAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Demo_Family";

        function GetFilteredFamilies(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredFamilies"), input);
        }
        function GetFamilyById(familyId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFamilyById"),
                {
                    familyId: familyId
                });
        }

        function UpdateFamily(family) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateFamily"), family);
        }
        function AddFamily(family) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddFamily"), family);
        };

        return {
            GetFilteredFamilies: GetFilteredFamilies,
            GetFamilyById: GetFamilyById,
            UpdateFamily: UpdateFamily,
            AddFamily: AddFamily,
        };
    };
    appControllers.service("Demo_Module_FamilyAPIService", familyAPIService);

})(appControllers);