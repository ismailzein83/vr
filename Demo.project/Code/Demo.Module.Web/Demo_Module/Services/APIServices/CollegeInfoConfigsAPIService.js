
(function (appControllers) {

    'use strict';

    CollegeInfoConfigsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];

    function CollegeInfoConfigsAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = 'CollegeInfoConfigs';

        function GetCollegeInfoTypeTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetCollegeInfoTypeTemplateConfigs'));
        }

        return ({
            GetCollegeInfoTypeTemplateConfigs: GetCollegeInfoTypeTemplateConfigs,
        });
    }


    appControllers.service('Demo_Module_CollegeInfoConfigsAPIService', CollegeInfoConfigsAPIService);
})(appControllers);