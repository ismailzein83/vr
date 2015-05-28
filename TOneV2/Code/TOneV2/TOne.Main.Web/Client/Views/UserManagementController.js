UserManagementController.$inject = ['$scope', '$q', 'UsersAPIService', 'VRModalService', 'VRNotificationService'];


function UserManagementController($scope, $q, UsersAPIService, VRModalService, VRNotificationService) {

    var mainGridAPI;
    var arrMenuAction = [];
    var stopPaging;

    defineScope();
    load();

    function defineScope() {

        $scope.gridMenuActions = [];

        $scope.users = [];

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };

        $scope.loadMoreData = function () {
            if (stopPaging)
                return;
            return getData();
        }

        $scope.searchClicked = function () {
            stopPaging = false;
            return getData(true);
        };

        $scope.AddNewUser = function () {

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "New User";
                modalScope.onUserAdded = function (user) {
                    mainGridAPI.itemAdded(user);
                };
            };
            VRModalService.showModal('/Client/Modules/Main/Views/UserEditor.html', null, settings);

        }
    }

    function load() {
        function MenuAction(name, width, title, url) {
            this.name = name;
            this.clicked = function (dataItem) {
                var params = {
                    userId: dataItem.UserId
                };

                var settings = {
                    width: width
                };

                settings.onScopeReady = function (modalScope) {
                    modalScope.title = title;
                    modalScope.onUserUpdated = function (user) {
                        mainGridAPI.itemUpdated(user);
                    };
                };
                VRModalService.showModal(url, params, settings);
            };
        }

        arrMenuAction.push(new MenuAction("Edit", "40%", "Edit User", "/Client/Modules/Main/Views/UserEditor.html"));
        arrMenuAction.push(new MenuAction("Reset Password", "40%", "Reset Password", "/Client/Modules/Main/Views/ResetPasswordEditor.html"));
        arrMenuAction.push(new MenuAction("Roles", "40%", "Roles", "/Client/Modules/Main/Views/RolesEditor.html"));

        arrMenuAction.forEach(function (item) {
            $scope.gridMenuActions.push({
                name: item.name,
                clicked: item.clicked
            });

        });

        getData(true);
    }

    function getData(startFromFirstRow) {
        var fromRow;
        if (startFromFirstRow) {
            fromRow = 1;
            $scope.users.length = 0;
        }
        else
            fromRow = $scope.users.length + 1;
        var toRow = fromRow + 20 - 1;

        var name = $scope.name != undefined ? $scope.name : '';
        var email = $scope.email != undefined ? $scope.email : '';
        return UsersAPIService.GetFilteredUsers(fromRow, toRow, name, email).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.users.push(itm);
            });
            if (response.length < 20)
                stopPaging = true;
        });
    }
}
appControllers.controller('UserManagementController', UserManagementController);