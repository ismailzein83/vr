
(function (appControllers) {

    'use stict';

    StatusChargingSetService.$inject = ['VRModalService'];

    function StatusChargingSetService(vrModalService) {

        function addStatusChargingSet(onStatusChargingSetAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onStatusChargingSetAdded = onStatusChargingSetAdded;
            };
            vrModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/StatusChargingSet/StatusChargingSetEditor.html', null, settings);
        };
        return {
            addStatusChargingSet: addStatusChargingSet
        };
    }

    appControllers.service('Retail_BE_StatusChargingSetService', StatusChargingSetService);

})(appControllers);