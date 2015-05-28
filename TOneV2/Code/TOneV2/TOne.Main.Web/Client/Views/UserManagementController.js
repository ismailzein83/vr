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
            getData();
        };

        $scope.loadMoreData = function () {
            return getData();
        }

        $scope.searchClicked = function () {
            mainGridAPI.clearDataAndContinuePaging();
            return getData();
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
    }

    function getData() {
        var pageInfo = mainGridAPI.getPageInfo();

        var name = $scope.name != undefined ? $scope.name : '';
        var email = $scope.email != undefined ? $scope.email : '';
        return UsersAPIService.GetFilteredUsers(pageInfo.fromRow, pageInfo.toRow, name, email).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.users.push(itm);
            });
            if (response.length < 20)
                stopPaging = true;
        });
    }
}
appControllers.controller('UserManagementController', UserManagementController);