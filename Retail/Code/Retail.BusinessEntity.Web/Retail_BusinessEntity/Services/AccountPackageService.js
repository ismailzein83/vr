(function (appControllers) {

    'use stict';

    AccountPackageService.$inject = ['VRModalService', 'VRNotificationService'];

    function AccountPackageService(VRModalService, VRNotificationService)
    {
        function assignPackageToAccount(accountBEDefinitionId, accountId, onAccountPackageAdded)
        {
            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountPackageAdded = onAccountPackageAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountPackage/PackageAssignmentEditor.html', parameters, settings);
        };

        return {
            assignPackageToAccount: assignPackageToAccount
        };
    }

    appControllers.service('Retail_BE_AccountPackageService', AccountPackageService);

})(appControllers);