(function (appControllers) {
    "use strict";
   familyAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function familyAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Demo_Family";

        function GetFilteredFamilies(input) {
            console.log("get")
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
            console.log(family)
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddFamily"), family);
        };
        function GetFamiliesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFamiliesInfo"), { filter: filter });
        };

        return {
            GetFilteredFamilies: GetFilteredFamilies,
            GetFamilyById: GetFamilyById,
            UpdateFamily: UpdateFamily,
            AddFamily: AddFamily,
            GetFamiliesInfo: GetFamiliesInfo,
        };
    };
    appControllers.service("Demo_Module_FamilyAPIService", familyAPIService);

})(appControllers);