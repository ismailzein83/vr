
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
        function editStatusChargingSet(statusChargingSetId, onStatusChargingSetUpdated) {
            var settings = {};

            var parameters = {
                statusChargingSetId: statusChargingSetId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onStatusChargingSetUpdated = onStatusChargingSetUpdated;
            };
            vrModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/StatusChargingSet/StatusChargingSetEditor.html', parameters, settings);
        }
        return {
            addStatusChargingSet: addStatusChargingSet,
            editStatusChargingSet: editStatusChargingSet
        };
    }

    appControllers.service('Retail_BE_StatusChargingSetService', StatusChargingSetService);

})(appControllers);