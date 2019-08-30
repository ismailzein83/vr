(function (appControllers) {

    'use strict';

    zooSectionService.$inject = ['VRModalService'];

    function zooSectionService(VRModalService) {

        function addZooSection(onZooSectionAdded, zooIdItem) {
            var parameters = {
                zooIdItem: zooIdItem
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onZooSectionAdded = onZooSectionAdded;
            };

            VRModalService.showModal('/Client/Modules/Demo_Module/Elements/ZooSection/Views/ZooSectionEditor.html', parameters, settings);
        }

        function editZooSection(onZooSectionUpdated, zooSectionId, zooIdItem) {
            var parameters = {
                zooSectionId: zooSectionId,
                zooIdItem: zooIdItem
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onZooSectionUpdated = onZooSectionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Demo_Module/Elements/ZooSection/Views/ZooSectionEditor.html', parameters, settings);
        }

        return {
            addZooSection: addZooSection,
            editZooSection: editZooSection
        };
    }

    appControllers.service('Demo_Module_ZooSectionService', zooSectionService);
})(appControllers);