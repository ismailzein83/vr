
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

        function GetDIDRuntimeEditor(dIDId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetDIDRuntimeEditor'), {
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

        function GetAccountDIDRelationDefinitionId() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountDIDRelationDefinitionId"));
        }

        function IsDIDAssignedToParentWithoutEED(accountDIDRelationDefinitionId, childId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "IsDIDAssignedToParentWithoutEED"), {
                accountDIDRelationDefinitionId: accountDIDRelationDefinitionId,
                childId: childId
            });
        }

        function GetAccountDIDRelationDefinition() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountDIDRelationDefinition"));
        }

        return ({
            GetFilteredDIDs: GetFilteredDIDs,
            GetDID: GetDID,
            AddDID: AddDID,
            HasAddDIDPermission: HasAddDIDPermission,
            UpdateDID: UpdateDID,
            HasUpdateDIDPermission: HasUpdateDIDPermission,
            GetDIDsInfo: GetDIDsInfo,
            GetAccountDIDRelationDefinitionId: GetAccountDIDRelationDefinitionId,
            IsDIDAssignedToParentWithoutEED: IsDIDAssignedToParentWithoutEED,
            GetDIDRuntimeEditor: GetDIDRuntimeEditor,
            GetAccountDIDRelationDefinition: GetAccountDIDRelationDefinition
        });
    }

    appControllers.service('Retail_BE_DIDAPIService', DIDAPIService);

})(appControllers);