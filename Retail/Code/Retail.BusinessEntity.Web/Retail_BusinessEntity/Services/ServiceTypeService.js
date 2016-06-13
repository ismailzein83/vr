(function (appControllers) {

    'use stict';

    ServiceTypeService.$inject = ['VRModalService', 'VRNotificationService'];

    function ServiceTypeService(VRModalService, VRNotificationService) {
        function editServiceType(serviceTypeId, parentServiceTypeId, onServiceTypeUpdated) {
            var parameters = {
                serviceTypeId: serviceTypeId,
                parentServiceTypeId: parentServiceTypeId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onServiceTypeUpdated = onServiceTypeUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/ServiceType/ServiceTypeEditor.html', parameters, settings);
        };



        function editPartType(partEntity, onPartTypeUpdated) {
            var parameters = {
                partEntity: partEntity,
            };
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onPartTypeUpdated = onPartTypeUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/ServiceType/ChargingPolicyPartEditor.html', parameters, settings);
        };

        function addPartType(onPartTypeAdded) {
            var parameters = {
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onPartTypeAdded = onPartTypeAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/ServiceType/ChargingPolicyPartEditor.html', parameters, settings);
        };
        return {
            editServiceType: editServiceType,
            addPartType: addPartType,
            editPartType:editPartType
        };
    }

    appControllers.service('Retail_BE_ServiceTypeService', ServiceTypeService);

})(appControllers);