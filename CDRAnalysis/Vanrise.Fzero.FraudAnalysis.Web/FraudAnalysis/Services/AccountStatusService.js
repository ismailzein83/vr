
app.service('Fzero_FraudAnalysis_AccountStatusService', ['VRModalService', 'VRNotificationService', 'UtilsService', 'Fzero_FraudAnalysis_AccountStatusAPIService',
    function (VRModalService, VRNotificationService, UtilsService, Fzero_FraudAnalysis_AccountStatusAPIService) {
        var drillDownDefinitions = [];
        return ({
            editAccountStatus: editAccountStatus,
            addAccountStatus: addAccountStatus,
            uploadAccountStatuses: uploadAccountStatuses,
            deleteAccountStatus: deleteAccountStatus

        });
        function editAccountStatus(accountNumber, onAccountStatusUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountStatusUpdated = onAccountStatusUpdated;
            };
            var parameters = {
                AccountNumber: accountNumber
            };

            VRModalService.showModal('/Client/Modules/FraudAnalysis/Views/AccountStatus/AccountStatusEditor.html', parameters, settings);
        }
        function addAccountStatus(onAccountStatusAdded) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountStatusAdded = onAccountStatusAdded;
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/FraudAnalysis/Views/AccountStatus/AccountStatusEditor.html', parameters, settings);
        }

        function deleteAccountStatus(accountStatusObj, onAccountStatusDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response == true) {
                        return Fzero_FraudAnalysis_AccountStatusAPIService.DeleteAccountStatus(accountStatusObj.Entity.AccountNumber)
                        .then(function (deletionResponse) {
                            if (VRNotificationService.notifyOnItemDeleted("AccountStatus", deletionResponse))
                                onAccountStatusDeleted(accountStatusObj);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
                });
        }

        function uploadAccountStatuses() {
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Upload White List";
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/FraudAnalysis/Views/AccountStatus/AccountStatusUploader.html', parameters, settings);
        }

    }]);
