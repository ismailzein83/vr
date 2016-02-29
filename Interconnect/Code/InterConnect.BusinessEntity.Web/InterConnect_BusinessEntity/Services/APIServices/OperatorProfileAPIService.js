(function (appControllers) {

    "use strict";
    operatorProfileAPIService.$inject = ['BaseAPIService', 'UtilsService', 'InterConnect_BE_ModuleConfig'];

    function operatorProfileAPIService(BaseAPIService, UtilsService, InterConnect_BE_ModuleConfig) {

        function GetFilteredOperatorProfiles(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(InterConnect_BE_ModuleConfig.moduleName, "OperatorProfile", "GetFilteredOperatorProfiles"), input);
        }

        function GetDataRecordTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(InterConnect_BE_ModuleConfig.moduleName, "OperatorProfile", "GetDataRecordTypes"));
        }

        function GetOperatorProfile(operatorProfileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(InterConnect_BE_ModuleConfig.moduleName, "OperatorProfile", "GetOperatorProfile"), {
                operatorProfileId: operatorProfileId
            });

        }

        function GetOperatorProfilsInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(InterConnect_BE_ModuleConfig.moduleName, "OperatorProfile", "GetOperatorProfilsInfo"), {
                serializedFilter: serializedFilter
            });

        }

        function GetRunTimeExtendedSettings(dataRecordTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(InterConnect_BE_ModuleConfig.moduleName, "OperatorProfile", "GetRunTimeExtendedSettings"), {
                dataRecordTypeId: dataRecordTypeId
            });

        }

        function UpdateOperatorProfile(operatorProfile) {
            return BaseAPIService.post(UtilsService.getServiceURL(InterConnect_BE_ModuleConfig.moduleName, "OperatorProfile", "UpdateOperatorProfile"), operatorProfile);
        }

        function AddOperatorProfile(operatorProfile) {
            return BaseAPIService.post(UtilsService.getServiceURL(InterConnect_BE_ModuleConfig.moduleName, "OperatorProfile", "AddOperatorProfile"), operatorProfile);
        }

        return ({
            GetDataRecordTypes:GetDataRecordTypes,
            GetRunTimeExtendedSettings:GetRunTimeExtendedSettings,
            GetOperatorProfilsInfo: GetOperatorProfilsInfo,
            GetFilteredOperatorProfiles: GetFilteredOperatorProfiles,
            GetOperatorProfile: GetOperatorProfile,
            AddOperatorProfile: AddOperatorProfile,
            UpdateOperatorProfile: UpdateOperatorProfile,
        });
    }

    appControllers.service('InterConnect_BE_OperatorProfileAPIService', operatorProfileAPIService);

})(appControllers);