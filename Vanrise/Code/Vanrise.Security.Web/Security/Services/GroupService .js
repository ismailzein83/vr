(function (appControllers) {

    'use strict';

    GroupService.$inject = ['VRModalService'];

    function GroupService(VRModalService) {
        return ({
            addGroup: addGroup,
            editGroup: editGroup
        });

        function addGroup(onGroupAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGroupAdded = onGroupAdded;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/Group/GroupEditor.html', null, modalSettings);
        }

        function editGroup(groupId, onGroupUpdated) {
            var modalParameters = {
                groupId: groupId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGroupUpdated = onGroupUpdated;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/Group/GroupEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VR_Sec_GroupService', GroupService);

})(appControllers);
