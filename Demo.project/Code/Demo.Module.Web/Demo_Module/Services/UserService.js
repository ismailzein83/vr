app.service('Demo_Module_UserService', ['VRModalService', 'Demo_Module_UserAPIService','VRNotificationService',
    function (VRModalService, Demo_Module_UserAPIService, VRNotificationService) {
        var drillDownDefinitions = [];
        return ({
            editUser: editUser,
            addUser: addUser,
            deleteUser:deleteUser

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
        function deleteUser(scope, dataItem, onUserDeleted) {
            VRNotificationService.showConfirmation().then(function (confirmed) {
                if (confirmed) {
                    return Demo_Module_UserAPIService.DeleteUser(dataItem.Entity.Id).then(function (responseObject) {
                        var deleted = VRNotificationService.notifyOnItemDeleted('User', responseObject);

                        if (deleted && onUserDeleted && typeof onUserDeleted == 'function') {
                            onUserDeleted(dataItem);
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, scope);
                    })
                }
            });
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