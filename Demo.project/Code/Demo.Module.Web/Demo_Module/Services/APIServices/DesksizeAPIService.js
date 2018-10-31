(function (appControllers) {
    "use strict";
    desksizeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function desksizeAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Desksize";

        function GetDesksizesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetDeskSizesInfo"), { filter: filter });
        };


        function GetDesksizeById(desksizeId) {

            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetDesksizeById"),
                {
                    desksizeId: desksizeId

                });
        }

        return {
            GetDesksizesInfo: GetDesksizesInfo,
            GetDesksizeById: GetDesksizeById
        };
    };
    appControllers.service("Demo_Module_DesksizeAPIService", desksizeAPIService);

})(appControllers);