(function (appControllers) {

    "use strict";

    VRLocalizationTextResourceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function VRLocalizationTextResourceAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        var controllerName = 'VRLocalizationTextResource';

        function GetFilteredVRLocalizationTextResources(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetFilteredVRLocalizationTextResources'), input);
        }

        function GetVRLocalizationTextResource(vrLocalizationTextResourceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetVRLocalizationTextResource'), {
                vrLocalizationTextResource: vrLocalizationTextResourceId
            });
        }

        function GetVRLocalizationTextResourceInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRLocalizationTextResourceInfo"), {
                filter: filter
            });
        }

        function AddVRLocalizationTextResource(vrLocalizationTextResourceItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "AddVRLocalizationTextResource"), vrLocalizationTextResourceItem);
        }

        function UpdateVRLocalizationTextResource(vrLocalizationTextResourceItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "UpdateVRLocalizationTextResource"), vrLocalizationTextResourceItem);
        }

        return ({
            GetFilteredVRLocalizationTextResources: GetFilteredVRLocalizationTextResources,
            GetVRLocalizationTextResource: GetVRLocalizationTextResource,
            GetVRLocalizationTextResourceInfo: GetVRLocalizationTextResourceInfo,
            AddVRLocalizationTextResource: AddVRLocalizationTextResource,
            UpdateVRLocalizationTextResource: UpdateVRLocalizationTextResource
        });

    }
    appControllers.service('VRCommon_VRLocalizationTextResourceAPIService', VRLocalizationTextResourceAPIService);

})(appControllers);