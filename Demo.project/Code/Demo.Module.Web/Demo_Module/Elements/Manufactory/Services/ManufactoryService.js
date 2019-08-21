(function (appControllers) {
    'use strict';

    manufactoryService.$inject = ['VRModalService', 'UtilsService'];

    function manufactoryService(VRModalService, UtilsService) {

        function addManufactory(onManufactoryAdded) {
            var parameters = {};

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onManufactoryAdded = onManufactoryAdded;
            };
            VRModalService.showModal('/Client/Modules/Demo_Module/Elements/Manufactory/Views/ManufactoryEditor.html', parameters, settings);
        }

        function editManufactory(onManufactoryUpdated, manufactoryId) {
            var parameters = {
                manufactoryId: manufactoryId
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onManufactoryUpdated = onManufactoryUpdated;
            };
            VRModalService.showModal('/Client/Modules/Demo_Module/Elements/Manufactory/Views/ManufactoryEditor.html', parameters, settings);
        }

        function viewManufactory(manufactoryId) {
            var parameters = {
                manufactoryId: manufactoryId,
                viewMode: true
            };
            
            VRModalService.showModal('/Client/Modules/Demo_Module/Elements/Manufactory/Views/ManufactoryEditor.html', parameters);
        }

        return {
            addManufactory: addManufactory,
            editManufactory: editManufactory,
            viewManufactory: viewManufactory
        };
    }

    appControllers.service('Demo_Module_ManufactoryService', manufactoryService);
})(appControllers);