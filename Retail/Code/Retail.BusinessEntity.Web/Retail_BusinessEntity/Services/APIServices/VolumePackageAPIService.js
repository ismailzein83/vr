(function (appControllers) {

    "use strict";
    VolumePackageAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig'];

    function VolumePackageAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig) {

        var controllerName = "VolumePackage";

        function GetCompositeRecordConditionResolvedDataList(volumePackageDefinitionId, volumePackageDefinitionItemId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetCompositeRecordConditionResolvedDataList"), {
                volumePackageDefinitionId: volumePackageDefinitionId,
                volumePackageDefinitionItemId: volumePackageDefinitionItemId
            });
        }

        return ({
            GetCompositeRecordConditionResolvedDataList: GetCompositeRecordConditionResolvedDataList
        });
    }

    appControllers.service('Retail_BE_VolumePackageAPIService', VolumePackageAPIService);
})(appControllers);