(function (appControllers) {

    'use stict';

    BalanceAlertService.$inject = ['VRModalService', 'VRNotificationService'];

    function BalanceAlertService(VRModalService, VRNotificationService) {
        function addBalanceAlertThreshold(onBalanceAlertThresholdAdded) {

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onBalanceAlertThresholdAdded = onBalanceAlertThresholdAdded
            };

            VRModalService.showModal('/Client/Modules/VR_AccountBalance/Views/BalanceAlertRule/BalanceAlertThresholdActionEditor.html', null, settings);
        };
        function editBalanceAlertThreshold(thresholdActionEntity, onBalanceAlertThresholdUpdated) {
            var settings = {};
            var parameters = {
                thresholdActionEntity: thresholdActionEntity,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onBalanceAlertThresholdUpdated = onBalanceAlertThresholdUpdated;
            };
            VRModalService.showModal('/Client/Modules/VR_AccountBalance/Views/BalanceAlertRule/BalanceAlertThresholdActionEditor.html', parameters, settings);
        }
        return {
            addBalanceAlertThreshold: addBalanceAlertThreshold,
            editBalanceAlertThreshold: editBalanceAlertThreshold
        };
    }

    appControllers.service('VR_AccountBalance_BalanceAlertService', BalanceAlertService);

})(appControllers);