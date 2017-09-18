(function (appControllers) {

    'use strict';

    DisputeCaseAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig'];

    function DisputeCaseAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig) {

        var controller = 'DisputeCase';

        function GetFilteredDisputeCases(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredDisputeCases"), input);
        }

        return ({

            GetFilteredDisputeCases: GetFilteredDisputeCases
        });
    }


    appControllers.service('Demo_Module_DisputeCaseAPIService', DisputeCaseAPIService);
})(appControllers);