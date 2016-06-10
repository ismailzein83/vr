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

        return {
            editServiceType: editServiceType
        };
    }

    appControllers.service('Retail_BE_ServiceTypeService', ServiceTypeService);

})(appControllers);