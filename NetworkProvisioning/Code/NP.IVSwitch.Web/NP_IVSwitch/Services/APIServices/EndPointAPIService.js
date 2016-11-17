
(function (appControllers) {

    "use strict";
    EndPointAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig', 'SecurityService'];

    function EndPointAPIService(BaseAPIService, UtilsService, NP_IVSwitch_ModuleConfig, SecurityService) {

        var controllerName = "EndPoint";


        function GetFilteredEndPoints(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetFilteredEndPoints'), input);
        }

        function GetEndPoint(EndPointId) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetEndPoint'), {
                EndPointId: EndPointId
            });
        }

        function AddEndPoint(EndPointItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'AddEndPoint'), EndPointItem);
        }

        function UpdateEndPoint(EndPointItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'UpdateEndPoint'), EndPointItem);
        }


        function HasAddEndPointPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['AddEndPoint']));
        }

        function HasEditEndPointPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['UpdateEndPoint']));
        }


        return ({
            GetFilteredEndPoints: GetFilteredEndPoints,
            GetEndPoint: GetEndPoint,
            AddEndPoint: AddEndPoint,
            UpdateEndPoint: UpdateEndPoint,
            HasAddEndPointPermission: HasAddEndPointPermission,
            HasEditEndPointPermission: HasEditEndPointPermission,
        });
    }

    appControllers.service('NP_IVSwitch_EndPointAPIService', EndPointAPIService);

})(appControllers);