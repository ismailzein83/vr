(function (appControllers) {

    "use strict";
    accountStatusAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRAnalysis_FA_ModuleConfig', 'SecurityService'];

    function accountStatusAPIService(BaseAPIService, UtilsService, CDRAnalysis_FA_ModuleConfig, SecurityService) {
        var controllerName = 'AccountStatus';

        function GetAccountStatusesData(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, "GetAccountStatusesData"), input);
        }

        function GetAccountStatus(accountNumber) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, "GetAccountStatus"), {
                accountNumber: accountNumber
            });

        }

        function UpdateAccountStatus(accountStatusObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, "UpdateAccountStatus"), accountStatusObject);
        }

        function AddAccountStatus(accountStatusObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, "AddAccountStatus"), accountStatusObject);
        }

        function DownloadAccountStatusesTemplate() {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, "DownloadAccountStatusesTemplate"),
                {},
                {
                    returnAllResponseParameters: true,
                    responseTypeAsBufferArray: true
                }
            );
        }

        function UploadAccountStatuses(fileId, validTill) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, "UploadAccountStatuses"),
                {
                    fileId: fileId,
                    validTill: validTill
                }

                );
        }

        function HasAddAccountStatusPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, ['AddAccountStatus']));
        }

        function HasUploadAccountStatusPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, ['UploadAccountStatuses']));
        }

        function HasDownloadAccountStatusPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, ['DownloadAccountStatusesTemplate']));
        }

        function HasEditAccountStatusPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, ['UpdateAccountStatus']));
        }

        return ({
            GetAccountStatusesData: GetAccountStatusesData,
            GetAccountStatus: GetAccountStatus,
            UpdateAccountStatus: UpdateAccountStatus,
            AddAccountStatus: AddAccountStatus,
            DownloadAccountStatusesTemplate: DownloadAccountStatusesTemplate,
            UploadAccountStatuses: UploadAccountStatuses,
            HasAddAccountStatusPermission: HasAddAccountStatusPermission,
            HasUploadAccountStatusPermission: HasUploadAccountStatusPermission,
            HasDownloadAccountStatusPermission: HasDownloadAccountStatusPermission,
            HasEditAccountStatusPermission: HasEditAccountStatusPermission
        });
    }

    appControllers.service('Fzero_FraudAnalysis_AccountStatusAPIService', accountStatusAPIService);

})(appControllers);