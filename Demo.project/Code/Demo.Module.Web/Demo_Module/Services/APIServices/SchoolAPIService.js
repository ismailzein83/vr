(function (appControllers) {
    "use strict";
    schoolAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function schoolAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "School";

        function GetFilteredSchools(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredSchools"), input);
        }

        function GetSchoolById(schoolId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetSchoolById"),
                {
                    schoolId: schoolId
                });
        }

        function UpdateSchool(school) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateSchool"), school);
        }

        function AddSchool(school) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddSchool"), school);
        };
        function GetSchoolsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetSchoolsInfo"), { filter: filter });
        };


        return {
            GetFilteredSchools: GetFilteredSchools,
            GetSchoolById: GetSchoolById,
            UpdateSchool: UpdateSchool,
            AddSchool: AddSchool,
            GetSchoolsInfo: GetSchoolsInfo,
        };
    };
    appControllers.service("Demo_Module_SchoolAPIService", schoolAPIService);

})(appControllers);