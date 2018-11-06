(function (appControllers) {
    "use strict";
    specialityAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function specialityAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Speciality";

        function GetSpecialitiesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetSpecialitiesInfo"), { filter: filter });
        };

        function GetSpecialityById(specialityId) {

            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetSpecialityById"),
                {
                    specialityId: specialityId

                });
        }

        return {
            GetSpecialitiesInfo: GetSpecialitiesInfo,
            GetSpecialityById: GetSpecialityById
        };
    };
    appControllers.service("Demo_Module_SpecialityAPIService", specialityAPIService);

})(appControllers);