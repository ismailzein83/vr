(function (appControllers) {

    'use stict';

    DestinationGroupService.$inject = ['VRModalService'];

    function DestinationGroupService(VRModalService) {
        return ({
            addDestinationGroup: addDestinationGroup,
            editDestinationGroup: editDestinationGroup
        });

        function addDestinationGroup(onDestinationGroupAdded) {
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onDestinationGroupAdded = onDestinationGroupAdded;
            };
            VRModalService.showModal('/Client/Modules/Demo_Module/Views/DestinationGroup/DestinationGroupEditor.html', null, settings);
        }

        function editDestinationGroup(destinationGroupObj, onDestinationGroupUpdated) {
            var modalSettings = {
            };

            var parameters = {
                DestinationGroupId: destinationGroupObj.Entity.DestinationGroupId,
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDestinationGroupUpdated = onDestinationGroupUpdated;
            };
            VRModalService.showModal('/Client/Modules/Demo_Module/Views/DestinationGroup/DestinationGroupEditor.html', parameters, modalSettings);
        }
    }

    appControllers.service('Demo_DestinationGroupService', DestinationGroupService);

})(appControllers);
