RoleManagementController.$inject = ['$scope', 'RoleAPIService', 'VRModalService'];


function RoleManagementController($scope, RoleAPIService, VRModalService) {
    var mainGridAPI;
    var arrMenuAction = [];
    var stopPaging;

    defineScope();
    load();

    function defineScope() {
        $scope.gridMenuActions = [];
        $scope.roles = [];

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

            settings.width = "40%";
            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Add Role";
                modalScope.onRoleAdded = function (role) {
                    mainGridAPI.itemAdded(role);
                };
            };
            VRModalService.showModal('/Client/Modules/Security/Views/RoleEditor.html', null, settings);
        }
    }

    function load() {
        function MenuAction(name, width, title, url) {
            this.name = name;
            this.clicked = function (dataItem) {
                var params = {
                    roleId: dataItem.RoleId
                };

                var settings = {
                    width: width
                };

                settings.onScopeReady = function (modalScope) {
                    modalScope.title = title;
                    modalScope.onRoleUpdated = function (role) {
                        mainGridAPI.itemUpdated(role);
                    };
                };
                VRModalService.showModal(url, params, settings);
            };
        }

        arrMenuAction.push(new MenuAction("Edit", "40%", "Edit Role", "/Client/Modules/Security/Views/RoleEditor.html"));

        arrMenuAction.forEach(function (item) {
            $scope.gridMenuActions.push({
                name: item.name,
                clicked: item.clicked
            });

        });


    }

    function getData()
    {
        var pageInfo = mainGridAPI.getPageInfo();

        var name = $scope.name != undefined ? $scope.name : '';
        return RoleAPIService.GetFilteredRoles(pageInfo.fromRow, pageInfo.toRow, name).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.roles.push(itm);
            });
            if (response.length < 20)
                stopPaging = true;
        });
    }
  
}
appControllers.controller('Security_RoleManagementController', RoleManagementController);