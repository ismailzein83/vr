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

    function defineScope(){
        $scope.validManagers = [];
        $scope.selectedManager = undefined;
        $scope.validUsers = [];
        $scope.selectedUsers = [];

        $scope.applyChanges = function () {
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

        $scope.treeReady = function (api) {
            treeAPI = api;
        }

        $scope.treeValueChanged = function () {
            if (angular.isObject($scope.currentNode) && $scope.currentNode != undefined) {

                setValidManagers();
                setValidUsers();
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
                    mapUsersToMembers();
                    $scope.isGettingData = false;
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            });
    }

    function setValidManagers() {

        // filter the valid managers
        $scope.validManagers = angular.copy(users); // reset the valid managers
        $scope.selectedManager = undefined; // reset the selected manager

        // remove the selected user and his/her members from the list of valid managers
        var memberIds = getMemberIds($scope.currentNode);
        memberIds = memberIds.concat($scope.currentNode.Id);

        for (var i = 0; i < memberIds.length; i++) {
            var memberIndex = UtilsService.getItemIndexByVal($scope.validManagers, memberIds[i], 'UserId');
            $scope.validManagers.splice(memberIndex, 1);
        }

        // select the manager of the selected user
        var managerId = findManager($scope.currentNode);

        if (managerId != 0) {
            var manager = UtilsService.getItemByVal(users, managerId, 'UserId');
            $scope.selectedManager = manager;
        }
    }

    function setValidUsers() {
        // filter the valid users
        $scope.validUsers = angular.copy(users); // reset the valid users

        // remove the invalid users from the list of invalid users
        var managerIds = getManagerIds(); // managerIds includes the id of the selected user

        for (var i = 0; i < managerIds.length; i++) {
            var managerIndex = UtilsService.getItemIndexByVal($scope.validUsers, managerIds[i], 'UserId');
            $scope.validUsers.splice(managerIndex, 1);
        }

        // select the members of the selected user
        $scope.selectedUsers.length = 0;
        for (var i = 0; i < $scope.currentNode.Members.length; i++) {
            var user = UtilsService.getItemByVal(users, $scope.currentNode.Members[i].Id, 'UserId');
            $scope.selectedUsers.push(user);
        }

        // keep track of the deselected users
        defaultSelectedUsers = angular.copy($scope.selectedUsers);
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

    function mapUsersToMembers() {
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
        addMissingMembers(); // in case the app's user has added new users to the database that weren't included in the org chart's hierarchy
        treeAPI.refreshTree(members);
    }

    function addMissingMembers() {
        var missingMembers = [];

        for (var i = 0; i < users.length; i++) {
            var member = findNode(users[i].UserId);

            if (member == undefined || member == null)
                missingMembers.push({
                    Id: users[i].UserId,
                    Name: users[i].Name,
                    Members: []
                });
        }

        members = members.concat(missingMembers);
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
            })
            .finally(function () {
                $scope.issaving = false;
            });
    }

    function updateOrgChart() {
        $scope.issaving = true;
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
            })
            .finally(function () {
                $scope.issaving = false;
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
        if (UtilsService.getItemIndexByVal(members, $scope.currentNode.Id, 'Id') != -1)
            return 0;

        for (var i = 0; i < members.length; i++) {
            var managerId = findManagerRecursively(members[i]);
            if (managerId != 0)
                return managerId;
        }
    }

    function findManagerRecursively(node) {
        if (node.Members.length == 0)
            return 0;
        
        if (UtilsService.getItemIndexByVal(node.Members, $scope.currentNode.Id, 'Id') != -1)
            return node.Id;

        for (var i = 0; i < node.Members.length; i++) {
            var managerId = findManagerRecursively(node.Members[i]);
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

    function getMemberIds(manager) {
        var ids = [];
        
        for (var i = 0; i < manager.Members.length; i++) {
            var currentMember = manager.Members[i];

            ids.push(currentMember.Id);

            if (currentMember.Members.length > 0)
                ids = ids.concat(getMemberIds(currentMember));
        }

        return ids;
    }

    function getManagerIds() { // including the id of the selected user

        if (UtilsService.getItemIndexByVal(members, $scope.currentNode.Id, 'Id') != -1)
            return [$scope.currentNode.Id]; // the selected user is a root user

        var ids = [];

        for (var i = 0; i < members.length; i++) {
            ids = getManagerIdsRecursively(members[i]);

            if (!UtilsService.contains(ids, -1)) // $scope.currentNode was found
                break;
        }

        return ids;
    }

    function getManagerIdsRecursively(node) {
        if (node.Members.length == 0)
            return [-1];

        if (UtilsService.getItemIndexByVal(node.Members, $scope.currentNode.Id, 'Id') != -1)
            return [node.Id, $scope.currentNode.Id];

        for (var i = 0; i < node.Members.length; i++) {
            var ids = [node.Id];
            ids = ids.concat(getManagerIdsRecursively(node.Members[i]));

            if (!UtilsService.contains(ids, -1)) // $scope.currentNode was found
                break;
        }

        return ids;
    }
}

appControllers.controller('Security_OrgChartEditorController', OrgChartEditorController);