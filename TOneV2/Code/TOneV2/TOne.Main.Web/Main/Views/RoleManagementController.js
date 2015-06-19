RoleManagementController.$inject = ['$scope', '$q', 'RolesAPIService', 'VRModalService', 'VRNotificationService'];


function RoleManagementController($scope, $q, RolesAPIService, VRModalService, VRNotificationService) {

    var mainGridAPI;
    var arrMenuAction = [];
    var stopPaging;

    defineScope();
    load();

    function defineScope() {

        $scope.gridMenuActions = [];

        $scope.Roles = [];

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

        $scope.AddNewRole = function () {

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "New Role";
                modalScope.onRoleAdded = function (Role) {
                    mainGridAPI.itemAdded(Role);
                };
            };
            VRModalService.showModal('/Client/Modules/Main/Views/RoleEditor.html', null, settings);

        }
    }

    function load() {
        function MenuAction(name, width, title, url) {
            this.name = name;
            this.clicked = function (dataItem) {
                var params = {
                    RoleId: dataItem.RoleId
                };

                var settings = {
                    width: width
                };

                settings.onScopeReady = function (modalScope) {
                    modalScope.title = title;
                    modalScope.onRoleUpdated = function (Role) {
                        mainGridAPI.itemUpdated(Role);
                    };
                };
                VRModalService.showModal(url, params, settings);
            };
        }

        arrMenuAction.push(new MenuAction("Edit", "40%", "Edit Role", "/Client/Modules/Main/Views/RoleEditor.html"));
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
        return RolesAPIService.GetFilteredRoles(pageInfo.fromRow, pageInfo.toRow, name, email).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.Roles.push(itm);
            });
            if (response.length < 20)
                stopPaging = true;
        });
    }
}
appControllers.controller('RoleManagementController', RoleManagementController);