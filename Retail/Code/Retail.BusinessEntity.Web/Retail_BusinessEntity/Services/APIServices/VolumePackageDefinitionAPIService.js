(function (appControllers) {

    "use strict";
    VolumePackageDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig'];

    function VolumePackageDefinitionAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig) {

        var controllerName = "VolumePackageDefinition";

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

    appControllers.service('Retail_BE_VolumePackageDefinitionAPIService', VolumePackageDefinitionAPIService);
})(appControllers);