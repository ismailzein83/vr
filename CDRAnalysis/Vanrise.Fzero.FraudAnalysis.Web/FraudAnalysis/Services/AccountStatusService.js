
app.service('Fzero_FraudAnalysis_AccountStatusService', ['VRModalService', 'VRNotificationService', 'UtilsService',
    function (VRModalService, VRNotificationService, UtilsService) {
        var drillDownDefinitions = [];
        return ({
            editAccountStatus: editAccountStatus,
            addAccountStatus: addAccountStatus,
            uploadAccountStatuses: uploadAccountStatuses

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

        function uploadAccountStatuses() {
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Upload AccountStatuses";
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/FraudAnalysis/Views/AccountStatus/AccountStatusUploader.html', parameters, settings);
        }

    }]);
