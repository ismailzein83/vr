
(function (appControllers) {

    "use strict";

    DIDAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function DIDAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = "DID";

        function GetFilteredDIDs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredDIDs'), input);
        }

        function GetDID(dIDId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetDID'), {
                dIDId: dIDId
            });
        }

        function AddDID(dID) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddDID'), dID);
        }

        function HasAddDIDPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['AddDID']));

        }

        function UpdateDID(dID) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateDID'), dID);
        }

        function HasUpdateDIDPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['UpdateDID']));
        }

        function GetDIDsInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetDIDsInfo"), {
                serializedFilter: serializedFilter
            });
        }

        return ({
            GetFilteredDIDs: GetFilteredDIDs,
            GetDID: GetDID,
            AddDID: AddDID,
            HasAddDIDPermission: HasAddDIDPermission,
            UpdateDID: UpdateDID,
            HasUpdateDIDPermission: HasUpdateDIDPermission,
            GetDIDsInfo: GetDIDsInfo
        });
    }

    appControllers.service('Retail_BE_DIDAPIService', DIDAPIService);

})(appControllers);