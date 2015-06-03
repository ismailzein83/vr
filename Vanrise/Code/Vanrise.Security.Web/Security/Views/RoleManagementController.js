RoleManagementController.$inject = ['$scope', 'RoleAPIService', 'VRModalService'];


function RoleManagementController($scope, RoleAPIService, VRModalService) {
    var mainGridAPI;
    var arrMenuAction = [];

    defineScope();
    load();

    function defineScope() {
        $scope.gridMenuActions = [];
        $scope.roles = [];

        defineMenuActions();

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

        $scope.AddNewRole = addRole;
    }

    function load() {
    }

    function getData()
    {
        var pageInfo = mainGridAPI.getPageInfo();

        var name = $scope.name != undefined ? $scope.name : '';
        return RoleAPIService.GetFilteredRoles(pageInfo.fromRow, pageInfo.toRow, name).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.roles.push(itm);
            });
        });
    }

    function defineMenuActions()
    {
        $scope.gridMenuActions = [{
            name: "Edit",
            clicked: editRole
        }];
    }

    function addRole() {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Add Role";
            modalScope.onRoleAdded = function (role) {
                mainGridAPI.itemAdded(role);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/RoleEditor.html', null, settings);
    }

    function editRole(roleObj)
    {
        var modalSettings = {
        };
        var parameters = {
            roleId: roleObj.RoleId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Role: " + roleObj.Name;
            modalScope.onRoleUpdated = function (role) {
                mainGridAPI.itemUpdated(role);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/RoleEditor.html', parameters, modalSettings);
    }
}
appControllers.controller('Security_RoleManagementController', RoleManagementController);