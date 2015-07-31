OrgChartEditorController.$inject = ['$scope', 'OrgChartAPIService', 'UsersAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function OrgChartEditorController($scope, OrgChartAPIService, UsersAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {

    var users = [];
    var members = [];
    var defaultSelectedUsers = [];

    var editMode;
    var treeAPI;
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
        $scope.validManagers = [];
        $scope.selectedManager = undefined;
        $scope.validUsers = [];
        $scope.selectedUsers = [];

        $scope.applyChange = function () {
            applyFunction();
            treeAPI.refreshTree(members);
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
                $scope.validUsers = angular.copy(users);

                // remove the selected user from the valid users
                var index = UtilsService.getItemIndexByVal($scope.validUsers, $scope.currentNode.Id, 'UserId');
                $scope.validUsers.splice(index, 1);

                // remove the selected manager from the valid users
                index = UtilsService.getItemIndexByVal($scope.validUsers, $scope.selectedManager.UserId, 'UserId');
                $scope.validUsers.splice(index, 1);
            }
        }

        $scope.treeReady = function (api) {
            treeAPI = api;
        }

        $scope.treeValueChanged = function () {
            if (angular.isObject($scope.currentNode) && $scope.currentNode != undefined) {
                if (angular.isObject($scope.currentNode)) {
                    // select the members of the selected user
                    $scope.selectedUsers.length = 0;
                    for (var i = 0; i < $scope.currentNode.Members.length; i++) {
                        var user = UtilsService.getItemByVal(users, $scope.currentNode.Members[i].Id, 'UserId');
                        $scope.selectedUsers.push(user);
                    }

                    // keep track of the deselected users
                    defaultSelectedUsers = angular.copy($scope.selectedUsers);

                    var managerId = findManager($scope.currentNode);
                    var manager = UtilsService.getItemByVal(users, managerId, 'UserId');
                    $scope.selectedManager = manager;

                    // remove the selected user from the managers selection menu
                    $scope.validManagers = angular.copy(users);
                    for (var i = 0; i < $scope.validManagers.length; i++) {
                        if ($scope.validManagers[i].UserId == $scope.currentNode.Id) {
                            $scope.validManagers.splice(i, 1);
                        }
                    }

                    // remove the selected user from the users selection menu
                    $scope.validUsers = angular.copy(users);
                    for (var i = 0; i < $scope.validUsers.length; i++) {
                        if ($scope.validUsers[i].UserId == $scope.currentNode.Id) { // remove the selected user
                            $scope.validUsers.splice(i, 1);
                        }
                    }

                    if (managerId != 0) {
                        var index = UtilsService.getItemIndexByVal($scope.validUsers, managerId, 'UserId');
                        if (index != -1)
                            $scope.validUsers.splice(index, 1);
                    }
                }
            }
        }
    }

    function load() {
        $scope.isGettingData = true;

        UsersAPIService.GetUsers()
            .then(function (response) {
                users = response;
                $scope.validUsers = users;
                $scope.validManagers = users;

                if (editMode)
                    getOrgChart(); // uses users at some point
                else {
                    setMembers();
                    $scope.isGettingData = false;
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            });
    }

    function getOrgChart() {

        return OrgChartAPIService.GetOrgChartById($scope.orgChartId)
            .then(function (response) {
                fillScopeFromOrgChartObj(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isGettingData = false;
            });
    }

    function setMembers() {
        // set members
        for (var i = 0; i < users.length; i++) {
            var member = {
                Id: users[i].UserId,
                Name: users[i].Name,
                Members: []
            }

            members.push(member);
            treeAPI.refreshTree(members);
        }
    };

    function buildOrgChartObjFromScope() {
        var orgChartObject = {
            Id: ($scope.orgChartId != null) ? $scope.orgChartId : 0,
            Name: $scope.name,
            Hierarchy: mapToServer(members)
        };
        return orgChartObject;
    }

    function fillScopeFromOrgChartObj(orgChartObject) {
        $scope.name = orgChartObject.Name;
        members = mapToClient(orgChartObject.Hierarchy);
        treeAPI.refreshTree(members);
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
                Name: UtilsService.getItemByVal(users, array[i].Id, 'UserId').Name,
                Members: mapToClient(array[i].Members),
                isOpened: true
            };

            temp.push(obj);
        }

        return temp;
    }

    function findManager() {
        for (var i = 0; i < members.length; i++) {
            var managerId = findManagerRecursively(members[i], $scope.currentNode);
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
        for (var i = 0; i < members.length; i++) {
            var node = getNodeRecursively(id, members[i]);
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
            removeNode(members, 0, userNode.Id);

            if (managerId == 0)
                members.push(userNode);
            else
                addNode(members, 0, userNode, managerId);
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
            for (var i = 0; i < defaultSelectedUsers.length; i++) {
                if (!UtilsService.contains($scope.selectedUsers, defaultSelectedUsers[i]))
                    reIndexNodes([defaultSelectedUsers[i].UserId], 0); // 0 => root
            }

            var userIds = getUserIds();
            reIndexNodes(userIds, $scope.currentNode.Id);
        }

        if ($scope.selectedManager != undefined) {
            var userIds = [$scope.currentNode.Id];
            reIndexNodes(userIds, $scope.selectedManager.UserId);
        }
    }
}

appControllers.controller('Security_OrgChartEditorController', OrgChartEditorController);