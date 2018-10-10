(function (appControllers) {

    "use strict";
    carrierProfileAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig', 'SecurityService'];

    function carrierProfileAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {

        var controllerName = "CarrierProfile";

        function GetFilteredCarrierProfiles(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredCarrierProfiles"), input);
        }

        function GetCarrierProfile(carrierProfileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetCarrierProfile"), {
                carrierProfileId: carrierProfileId
            });
        }

        function GetCarrierProfilesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetCarrierProfilesInfo"));

        }

        function GetTaxesDefinition() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetTaxesDefinition"));
        }

        function UpdateCarrierProfile(carrierProfileObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "UpdateCarrierProfile"), carrierProfileObject);
        }

        function AddCarrierProfile(carrierProfileObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "AddCarrierProfile"), carrierProfileObject);
        }

        function HasUpdateCarrierProfilePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['UpdateCarrierProfile']));
        }

        function HasAddCarrierProfilePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['AddCarrierProfile']));
        }
        function GetCarrierProfileHistoryDetailbyHistoryId(carrierProfileHistoryId) {

            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetCarrierProfileHistoryDetailbyHistoryId'), {
                carrierProfileHistoryId: carrierProfileHistoryId
            });
        }
        function GetCarrierProfileTicketContactsInfo(serializedTicketContactInfoFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetCarrierProfileTicketContactsInfo"), {
                serializedTicketContactInfoFilter: serializedTicketContactInfoFilter
            });
        }
        function GetAccountManagerName(accountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetAccountManagerName"), { accountId: accountId });
        }
        return ({
            GetCarrierProfilesInfo: GetCarrierProfilesInfo,
            GetFilteredCarrierProfiles: GetFilteredCarrierProfiles,
            GetCarrierProfile: GetCarrierProfile,
            AddCarrierProfile: AddCarrierProfile,
            UpdateCarrierProfile: UpdateCarrierProfile,
            HasUpdateCarrierProfilePermission: HasUpdateCarrierProfilePermission,
            HasAddCarrierProfilePermission: HasAddCarrierProfilePermission,
            GetTaxesDefinition: GetTaxesDefinition,
            GetCarrierProfileHistoryDetailbyHistoryId: GetCarrierProfileHistoryDetailbyHistoryId,
            GetCarrierProfileTicketContactsInfo: GetCarrierProfileTicketContactsInfo,
            GetAccountManagerName: GetAccountManagerName
        });
    }

    appControllers.service('WhS_BE_CarrierProfileAPIService', carrierProfileAPIService);

})(appControllers);