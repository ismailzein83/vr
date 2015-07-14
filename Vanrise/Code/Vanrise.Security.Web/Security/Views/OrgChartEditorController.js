OrgChartEditorController.$inject = ['$scope', 'OrgChartAPIService', 'UsersAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function OrgChartEditorController($scope, OrgChartAPIService, UsersAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {

    var editMode;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        $scope.orgChartId = undefined;

        if (parameters != undefined && parameters != null) {
            $scope.orgChartId = parameters.orgChartId;
        }

        if ($scope.orgChartId != undefined)
            editMode = true;
        else {
            editMode = false;
            $scope.name = ''; // hides the save button
        }
    }

    function defineScope() {
        $scope.members = [];
        $scope.validManagers = [];
        $scope.selectedManager = undefined;
        $scope.users = [];
        $scope.validUsers = [];
        $scope.selectedUsers = [];
        $scope.defaultSelectedUsers = [];

        $scope.applyChange = function () {
            applyFunction();
        }

        $scope.saveOrgChart = function () {
            if (editMode)
                return updateOrgChart();
            else
                return insertOrgChart();
        };

        $scope.close = function () {
            $scope.modalContext.closeModal();
        };

        $scope.filterUsers = function () {
            if ($scope.selectedManager != undefined) {
                // reset the valid users
                $scope.validUsers = angular.copy($scope.users);

                // remove the selected user from the valid users
                var index = UtilsService.getItemIndexByVal($scope.validUsers, $scope.tree.currentNode.Id, 'UserId');
                $scope.validUsers.splice(index, 1);

                // remove the selected manager from the valid users
                index = UtilsService.getItemIndexByVal($scope.validUsers, $scope.selectedManager.UserId, 'UserId');
                $scope.validUsers.splice(index, 1);
            }
        }

        //$scope.filterManagers = function () {
        //    // reset the valid valid managers
        //    $scope.validManagers = angular.copy($scope.users);

        //    // remove the selected user from the valid managers
        //    var index = UtilsService.getItemIndexByVal($scope.validManagers, $scope.tree.currentNode.Id, 'UserId');
        //    $scope.validManagers.splice(index, 1);

        //    // remove the members of the selected users from the valid managers

        //}

        $scope.$watch('tree.currentNode', function (newObj, oldObj) {
            if (angular.isObject($scope.tree.currentNode)) {
                // select the members of the selected user
                $scope.selectedUsers.length = 0;
                for (var i = 0; i < newObj.Members.length; i++) {
                    var user = UtilsService.getItemByVal($scope.users, newObj.Members[i].Id, 'UserId');
                    $scope.selectedUsers.push(user);
                }

                // keep track of the deselected users
                $scope.defaultSelectedUsers = angular.copy($scope.selectedUsers);

                var managerId = findManager(newObj);
                var manager = UtilsService.getItemByVal($scope.users, managerId, 'UserId');
                $scope.selectedManager = manager;

                // remove the selected user from the managers selection menu
                $scope.validManagers = angular.copy($scope.users);
                for (var i = 0; i < $scope.validManagers.length; i++) {
                    if ($scope.validManagers[i].UserId == newObj.Id) {
                        $scope.validManagers.splice(i, 1);
                    }
                }

                // remove the selected user from the users selection menu
                $scope.validUsers = angular.copy($scope.users);
                for (var i = 0; i < $scope.validUsers.length; i++) {
                    if ($scope.validUsers[i].UserId == newObj.Id) { // remove the selected user
                        $scope.validUsers.splice(i, 1);
                    }
                }

                if (managerId != 0) {
                    var index = UtilsService.getItemIndexByVal($scope.validUsers, managerId, 'UserId');
                    if (index != -1)
                        $scope.validUsers.splice(index, 1);
                }
            }
        }, false);
    }

    function load() {
        UtilsService.waitMultipleAsyncOperations([loadUsers]).finally(function () {
            if (editMode) {
                $scope.isGettingData = true;

                getOrgChart().finally(function () {
                    $scope.isGettingData = false;
                });
            }
            else {
                setMembers();
            }
        });
    }

    function loadUsers() {
        return UsersAPIService.GetUsers().then(function (response) {
            $scope.users = response;
            $scope.validUsers = $scope.users;
            $scope.validManagers = $scope.users;
        });
    }

    function setMembers() {
        // set $scope.members
        for (var i = 0; i < $scope.users.length; i++) {
            var member = {
                Id: $scope.users[i].UserId,
                Name: $scope.users[i].Name,
                Members: []
            }

            $scope.members.push(member);
        }
    };

    function getOrgChart() {
        return OrgChartAPIService.GetOrgChartById($scope.orgChartId)
            .then(function (response) {
                fillScopeFromOrgChartObj(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }

    function buildOrgChartObjFromScope() {
        var orgChartObject = {
            Id: ($scope.orgChartId != null) ? $scope.orgChartId : 0,
            Name: $scope.name,
            Hierarchy: mapToServer($scope.members)
        };
        return orgChartObject;
    }

    function fillScopeFromOrgChartObj(orgChartObject) {
        $scope.name = orgChartObject.Name;
        $scope.members = mapToClient(orgChartObject.Hierarchy);
    }

    function insertOrgChart() {
        $scope.issaving = true;
        var orgChartObject = buildOrgChartObjFromScope();

        return OrgChartAPIService.AddOrgChart(orgChartObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("OrgChart", response)) {
                    if ($scope.onOrgChartAdded != undefined)
                        $scope.onOrgChartAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function updateOrgChart() {
        var orgChartObject = buildOrgChartObjFromScope();

        OrgChartAPIService.UpdateOrgChart(orgChartObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("OrgChart", response)) {
                if ($scope.onOrgChartUpdated != undefined)
                    $scope.onOrgChartUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        })
        .catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }

    function mapToServer(array) {
        if (array.length == 0)
            return [];

        var temp = [];

        for (var i = 0; i < array.length; i++) {
            var obj = {
                Id: array[i].Id,
                Members: mapToServer(array[i].Members)
            };

            temp.push(obj);
        }

        return temp;
    }

    function mapToClient(array) {
        if (array.length == 0)
            return [];

        var temp = [];

        for (var i = 0; i < array.length; i++) {
            var obj = {
                Id: array[i].Id,
                Name: UtilsService.getItemByVal($scope.users, array[i].Id, 'UserId').Name,
                Members: mapToClient(array[i].Members)
            };

            temp.push(obj);
        }

        return temp;
    }

    function findManager() {
        for (var i = 0; i < $scope.members.length; i++) {
            var managerId = findManagerRecursively($scope.members[i], $scope.tree.currentNode);
            if (managerId != 0)
                return managerId;
        }
        return 0;
    }

    function findManagerRecursively(item, childMember) {
        if (item.Members.length == 0)
            return 0;
        
        if (UtilsService.contains(item.Members, childMember))
            return item.Id;

        for (var i = 0; i < item.Members.length; i++) {
            var managerId = findManagerRecursively(item.Members[i], childMember);
            if (managerId != 0)
                return managerId;
        }
        return 0;
    }

    function findNode(id) {
        for (var i = 0; i < $scope.members.length; i++) {
            var node = getNodeRecursively(id, $scope.members[i]);
            if (node != null)
                return node;
        }

        return null;
    }

    function getNodeRecursively(id, node) {
        if (node.Id == id)
            return node;

        if (node.Members != null && node.Members.length > 0) {
            for (var i = 0; i < node.Members.length; i++) {
                var result = getNodeRecursively(id, node.Members[i]);
                if (result != null)
                    return result;
            }
        }

        return null;
    }

    function removeNode(array, i, removeId) {
        if (i == array.length)
            return;
        else if (array[i].Id == removeId) {
            array.splice(i, 1)[0];
            return;
        }
        else {
            removeNode(array, i + 1, removeId);

            if (array[i].Members.length > 0) {
                removeNode(array[i].Members, 0, removeId);
            }
        }
    }

    function addNode(array, i, node, managerId) {
        if (i == array.length)
            return;
        else if (array[i].Id == managerId) {
            array[i].Members.push(node);
            return;
        }
        else {
            addNode(array, i + 1, node, managerId);

            if (array[i].Members.length > 0) {
                addNode(array[i].Members, 0, node, managerId);
            }
        }
    }

    function reIndexNodes(userIds, managerId) {
        for (var i = 0; i < userIds.length; i++) {
            var userNode = findNode(userIds[i]);
            removeNode($scope.members, 0, userNode.Id);

            if (managerId == 0)
                $scope.members.push(userNode);
            else
                addNode($scope.members, 0, userNode, managerId);
        }
    };

    function getUserIds() {
        var ids = [];

        for (var i = 0; i < $scope.selectedUsers.length; i++) {
            ids.push($scope.selectedUsers[i].UserId);
        }

        return ids;
    }

    function applyFunction()
    {
        if ($scope.selectedUsers.length) {
            // add the deselected users to the root of the org chart
            for (var i = 0; i < $scope.defaultSelectedUsers.length; i++) {
                if (!UtilsService.contains($scope.selectedUsers, $scope.defaultSelectedUsers[i]))
                    reIndexNodes([$scope.defaultSelectedUsers[i].UserId], 0); // 0 => root
            }

            var userIds = getUserIds();
            reIndexNodes(userIds, $scope.tree.currentNode.Id);
        }

        if ($scope.selectedManager != undefined) {
            var userIds = [$scope.tree.currentNode.Id];
            reIndexNodes(userIds, $scope.selectedManager.UserId);
        }
    }
}

appControllers.controller('Security_OrgChartEditorController', OrgChartEditorController);