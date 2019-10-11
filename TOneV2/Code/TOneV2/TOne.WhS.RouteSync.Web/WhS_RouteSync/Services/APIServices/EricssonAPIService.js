(function (appControllers) {

    'use strict';

    EricssonAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_RouteSync_ModuleConfig'];

    function EricssonAPIService(BaseAPIService, UtilsService, WhS_RouteSync_ModuleConfig) {

        var controllerName = 'Ericsson';

        function IsSupplierTrunkUsed(supplierId, trunkId, trunkName, carrierMappings) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "IsSupplierTrunkUsed"), {
                SupplierId: supplierId,
                TrunkId: trunkId,
                TrunkName: trunkName,
                CarrierMappings: carrierMappings
            });
        }

        return {
            IsSupplierTrunkUsed: IsSupplierTrunkUsed
        };
    }

    appControllers.service('WhS_RouteSync_EricssonAPIService', EricssonAPIService);
})(appControllers);