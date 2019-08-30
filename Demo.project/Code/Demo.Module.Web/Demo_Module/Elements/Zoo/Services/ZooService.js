(function (appControllers) {

    'use strict';

    zooService.$inject = ['VRModalService'];

    function zooService(VRModalService) {

        function addZoo(onZooAdded) {

            var parameters = {};

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onZooAdded = onZooAdded;
            };

            VRModalService.showModal('/Client/Modules/Demo_Module/Elements/Zoo/Views/ZooEditor.html', parameters, settings);
        }

        function editZoo(onZooUpdated, zooId) {

            var parameters = {
                zooId: zooId
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onZooUpdated = onZooUpdated;
            };

            VRModalService.showModal('/Client/Modules/Demo_Module/Elements/Zoo/Views/ZooEditor.html', parameters, settings);
        }

        return {
            addZoo: addZoo,
            editZoo: editZoo
        };
    }

    appControllers.service('Demo_Module_ZooService', zooService);
})(appControllers);
