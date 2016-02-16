(function (appControllers) {

    "use strict";
    operatorProfileAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_ModuleConfig'];

    function operatorProfileAPIService(BaseAPIService, UtilsService, Demo_ModuleConfig) {

        function GetFilteredOperatorProfiles(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "OperatorProfile", "GetFilteredOperatorProfiles"), input);
        }

        function GetOperatorProfile(operatorProfileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "OperatorProfile", "GetOperatorProfile"), {
                operatorProfileId: operatorProfileId
            });

        }
        function GetOperatorProfilesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "OperatorProfile", "GetOperatorProfilesInfo"));

        }
        function UpdateOperatorProfile(operatorProfileObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "OperatorProfile", "UpdateOperatorProfile"), operatorProfileObject);
        }
        function AddOperatorProfile(operatorProfileObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "OperatorProfile", "AddOperatorProfile"), operatorProfileObject);
        }
        return ({
            GetOperatorProfilesInfo: GetOperatorProfilesInfo,
            GetFilteredOperatorProfiles: GetFilteredOperatorProfiles,
            GetOperatorProfile: GetOperatorProfile,
            AddOperatorProfile:AddOperatorProfile,
            UpdateOperatorProfile: UpdateOperatorProfile
        });
    }

    appControllers.service('Demo_OperatorProfileAPIService', operatorProfileAPIService);

})(appControllers);