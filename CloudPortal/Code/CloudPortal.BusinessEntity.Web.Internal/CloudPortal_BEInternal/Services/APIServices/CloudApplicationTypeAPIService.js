(function (appControllers) {

    "use strict";
    BEInternal_CloudApplicationTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CloudPortal_BEInternal_ModuleConfig'];

    function BEInternal_CloudApplicationTypeAPIService(BaseAPIService, UtilsService, CloudPortal_BEInternal_ModuleConfig) {
        var prefix = 'CloudApplicationType';

        function GetFilteredCloudApplicationTypes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, "GetFilteredCloudApplicationTypes"), input);
        }

        function GetCloudApplicationType(id) {
            return BaseAPIService.get(UtilsService.getServiceURL(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, "GetCloudApplicationType"), {
                id: id
            });
        }

        function GetCloudApplicationTypesInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, "GetCloudApplicationTypesInfo"), {
                serializedFilter: serializedFilter
            });
        }

        function AddCloudApplicationType(cloudApplicationType) {
            return BaseAPIService.post(UtilsService.getServiceURL(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, "AddCloudApplicationType"), cloudApplicationType);
        }

        function UpdateCloudApplicationType(cloudApplicationType) {
            return BaseAPIService.post(UtilsService.getServiceURL(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, "UpdateCloudApplicationType"), cloudApplicationType);
        }

        return ({
            GetFilteredCloudApplicationTypes: GetFilteredCloudApplicationTypes,
            GetCloudApplicationType: GetCloudApplicationType,
            AddCloudApplicationType: AddCloudApplicationType,
            UpdateCloudApplicationType: UpdateCloudApplicationType,
            GetCloudApplicationTypesInfo: GetCloudApplicationTypesInfo
        });
    }

    appControllers.service('CloudPortal_BEInternal_CloudApplicationTypeAPIService', BEInternal_CloudApplicationTypeAPIService);

})(appControllers);