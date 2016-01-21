app.service('VR_Sec_GroupService', ['VRModalService',
    function (VRModalService) {

        function addGroup(onGroupAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onGroupAdded = onGroupAdded;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/Group/GroupEditor.html', null, settings);
        }

        function editGroup(groupId, onGroupUpdated) {
            var modalSettings = {};

            var parameters = {
                groupId: groupId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGroupUpdated = onGroupUpdated;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/Group/GroupEditor.html', parameters, modalSettings);
        }


        return ({
            addGroup: addGroup,
            editGroup: editGroup
        });

    }]);