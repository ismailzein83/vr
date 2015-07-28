GroupManagementController.$inject = ['$scope', 'GroupAPIService', 'VRModalService'];

function GroupManagementController($scope, GroupAPIService, VRModalService) {
    var mainGridAPI;
    var arrMenuAction = [];

    defineScope();
    load();

    function defineScope() {
        $scope.gridMenuActions = [];
        $scope.groups = [];

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

        $scope.addNewGroup = addGroup;
    }

    function load() {
    }

    function getData()
    {
        var pageInfo = mainGridAPI.getPageInfo();

        var name = $scope.name != undefined ? $scope.name : '';
        return GroupAPIService.GetFilteredGroups(pageInfo.fromRow, pageInfo.toRow, name).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.groups.push(itm);
            });
        });
    }

    function defineMenuActions()
    {
        $scope.gridMenuActions = [{
            name: "Edit",
            clicked: editGroup,
            permissions: "TOne/Administration Module/Groups:Edit"
        },
        {
            name: "Assign Permissions",
            clicked: assignPermissions,
            permissions: "TOne/Administration Module/System Entities:Assign Permissions"
        }
        ];
    }

    function addGroup() {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Add Group";
            modalScope.onGroupAdded = function (group) {
                mainGridAPI.itemAdded(group);
            };
        };

        VRModalService.showModal('/Client/Modules/Security/Views/GroupEditor.html', null, settings);
    }

    function editGroup(groupObj)
    {
        var modalSettings = {};

        var parameters = {
            groupId: groupObj.GroupId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Group: " + groupObj.Name;
            modalScope.onGroupUpdated = function (group) {
                mainGridAPI.itemUpdated(group);
            };
        };

        VRModalService.showModal('/Client/Modules/Security/Views/GroupEditor.html', parameters, modalSettings);
    }

    function assignPermissions(groupObj) {
        var modalSettings = {};

        var parameters = {
            holderType: 1,
            holderId: groupObj.GroupId,
            notificationResponseText: "Group Permissions"
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Assign Permissions to Group: " + groupObj.Name;
        };

        VRModalService.showModal('/Client/Modules/Security/Views/PermissionEditor.html', parameters, modalSettings);
    }
}

appControllers.controller('Security_GroupManagementController', GroupManagementController);