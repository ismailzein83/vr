GroupEditorController.$inject = ['$scope', 'GroupAPIService', 'UsersAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function GroupEditorController($scope, GroupAPIService, UsersAPIService, VRModalService, VRNotificationService, VRNavigationService) {

    var editMode;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.groupId = undefined;

        if (parameters != undefined && parameters != null)
            $scope.groupId = parameters.groupId;

        if ($scope.groupId != undefined)
            editMode = true;
        else
            editMode = false;
    }

    function defineScope() {
        $scope.saveGroup = function () {
            if (editMode) {
                return updateGroup();
            }
            else {
                return insertGroup();
            }
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.optionsUsers = {
            selectedvalues: [],
            datasource: []
        };
    }

    function load() {
        UsersAPIService.GetUsers().then(function (response) {
            $scope.optionsUsers.datasource = response;

        });

        if (editMode) {
            $scope.isGettingData = true;
            getGroup().finally(function () {
                $scope.isGettingData = false;
            })
        }
    }

    function getGroup() {
        return GroupAPIService.GetGroup($scope.groupId)
           .then(function (response) {
               fillScopeFromGroupObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }

    function buildGroupObjFromScope() {
        var selectedUserIds = [];
        angular.forEach($scope.optionsUsers.selectedvalues, function (user) {
            selectedUserIds.push(user.UserId);
        });

        var groupObj = {
            groupId: ($scope.groupId != null) ? $scope.groupId : 0,
            name: $scope.name,
            description: $scope.description,
            members: selectedUserIds
        };

        return groupObj;
    }

    function fillScopeFromGroupObj(groupObj) {
        $scope.name = groupObj.Name;
        $scope.description = groupObj.Description;
        
        UsersAPIService.GetMembers($scope.groupId).then(function (response) {
            $scope.optionsUsers.selectedvalues = response;
        });
    }

    function insertGroup() {
        $scope.issaving = true;
        var groupObj = buildGroupObjFromScope();

        return GroupAPIService.AddGroup(groupObj)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("Group", response)) {
                if ($scope.onGroupAdded != undefined)
                    $scope.onGroupAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });

    }

    function updateGroup() {
        var groupObj = buildGroupObjFromScope();

        GroupAPIService.UpdateGroup(groupObj)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Group", response)) {
                if ($scope.onGroupUpdated != undefined)
                    $scope.onGroupUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }
}

appControllers.controller('Security_GroupEditorController', GroupEditorController);
