GroupEditorController.$inject = ['$scope', 'GroupAPIService', 'UsersAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function GroupEditorController($scope, GroupAPIService, UsersAPIService, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {

    var editMode;
    var group;
    var members;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.groupId = undefined;
        
        if (parameters != undefined && parameters != null)
            $scope.groupId = parameters.groupId;

        editMode = ($scope.groupId != undefined);
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
        $scope.isGettingData = true;

        UsersAPIService.GetUsers().then(function (response) {
            $scope.optionsUsers.datasource = response;
            
            if (editMode) {
                UtilsService.waitMultipleAsyncOperations([getGroup, getMembers])
                    .then(function () {
                        fillScopeFromGroupAndMembersObjs();
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    })
                    .finally(function () {
                        $scope.isGettingData = false;
                    });
            }
            else {
                $scope.isGettingData = false;
            }

        }).catch(function (error) {
            $scope.isGettingData = false;
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
    }

    function getGroup() {
        return GroupAPIService.GetGroup($scope.groupId)
            .then(function (response) {
                group = response;
            });
    }

    function getMembers() {
        return UsersAPIService.GetMembers($scope.groupId)
            .then(function (response) {
                members = response;
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

    function fillScopeFromGroupAndMembersObjs() {
        $scope.name = group.Name;
        $scope.description = group.Description;
        $scope.optionsUsers.selectedvalues = members;
    }

    function insertGroup() {
        var groupObj = buildGroupObjFromScope();

        return GroupAPIService.AddGroup(groupObj)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Group", response)) {
                    if ($scope.onGroupAdded != undefined)
                        $scope.onGroupAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
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
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }
}

appControllers.controller('Security_GroupEditorController', GroupEditorController);
