(function (appControllers) {

    'use stict';

    BalanceAlertService.$inject = ['VRModalService', 'VRNotificationService'];

    function BalanceAlertService(VRModalService, VRNotificationService) {

        return {
         
        };
    }

    appControllers.service('VR_AccountBalance_BalanceAlertService', BalanceAlertService);

})(appControllers);