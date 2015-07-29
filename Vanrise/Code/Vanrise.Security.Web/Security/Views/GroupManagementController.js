GroupManagementController.$inject = ['$scope', 'GroupAPIService', 'VRModalService', 'VRNotificationService'];

function GroupManagementController($scope, GroupAPIService, VRModalService, VRNotificationService) {

    var gridApi;
    var arrMenuAction = [];

    defineScope();
    load();

    function defineScope() {
        $scope.groups = [];
        $scope.gridMenuActions = [];

        defineMenuActions();

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return GroupAPIService.GetFilteredGroups(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        $scope.gridReady = function (api) {
            gridApi = api;
            return retrieveData();
        };

        $scope.searchClicked = function () {
            return retrieveData();
        };

        $scope.addNewGroup = addGroup;
    }

    function load() {
    }

    function retrieveData() {
        var query = {
            Name: $scope.name
        };

        return gridApi.retrieveData(query);
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
                gridApi.itemAdded(group);
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
                gridApi.itemUpdated(group);
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