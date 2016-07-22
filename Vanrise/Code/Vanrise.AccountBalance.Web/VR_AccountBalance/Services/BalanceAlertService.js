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

        return {
            addBalanceAlertThreshold: addBalanceAlertThreshold,
        };
    }

    appControllers.service('VR_AccountBalance_BalanceAlertService', BalanceAlertService);

})(appControllers);