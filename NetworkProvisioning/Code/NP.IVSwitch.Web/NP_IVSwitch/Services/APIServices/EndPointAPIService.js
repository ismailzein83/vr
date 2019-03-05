﻿
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

        function GetEndPointsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetEndPointsInfo'), {
                filter: filter
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

        function GetEndPointHistoryDetailbyHistoryId(endPointHistoryId) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetEndPointHistoryDetailbyHistoryId'), {
                endPointHistoryId: endPointHistoryId
            });
        }

		function DeleteEndPoint(endPointId) {
			return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'DeleteEndPoint'), {
				endPointId: endPointId,
			});
		}

		function HasDeletePermisssion() {
			return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['DeleteEndPoint']));
		}
		function HasViewEndPointPermission() {
			return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['GetFilteredEndPoints']));
		}
        return ({
            GetFilteredEndPoints: GetFilteredEndPoints,
            GetEndPoint: GetEndPoint,
            AddEndPoint: AddEndPoint,
            UpdateEndPoint: UpdateEndPoint,
            HasAddEndPointPermission: HasAddEndPointPermission,
            HasEditEndPointPermission: HasEditEndPointPermission,
            GetEndPointsInfo: GetEndPointsInfo,
			GetEndPointHistoryDetailbyHistoryId: GetEndPointHistoryDetailbyHistoryId,
			DeleteEndPoint: DeleteEndPoint,
			HasDeletePermisssion: HasDeletePermisssion,
			HasViewEndPointPermission: HasViewEndPointPermission
        });
    }

    appControllers.service('NP_IVSwitch_EndPointAPIService', EndPointAPIService);

})(appControllers);