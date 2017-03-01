app.service('Demo_Module_UserService', ['VRModalService', 'Demo_Module_UserAPIService',
    function (VRModalService, VRNotificationService, UtilsService, Demo_Module_UserAPIService) {
        var drillDownDefinitions = [];
        return ({
            editUser: editUser,
            addUser: addUser,

        });
        function editUser(Id, onUserUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onUserUpdated = onUserUpdated;
            };
            var parameters = {
                Id: Id
            };

            VRModalService.showModal('/Client/Modules/Demo_Module/Views/UserEditor.html', parameters, settings);
        }
        function addUser(onUserAdded) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onUserAdded = onUserAdded;
            };
            var parameters = {};


            VRModalService.showModal('/Client/Modules/Demo_Module/Views/UserEditor.html', parameters, settings);
        }



       

    }]);