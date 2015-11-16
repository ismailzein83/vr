AccountManagerManagementController.$inject = ['$scope', 'WhS_BE_AccountManagerAPIService', 'UsersAPIService', 'OrgChartAPIService', 'ApplicationParameterAPIService', 'OrgChartAPIService', 'VRModalService', 'VRNotificationService', 'UtilsService','WhS_BE_MainService'];

function AccountManagerManagementController($scope, AccountManagerAPIService, UsersAPIService, OrgChartAPIService, ApplicationParameterAPIService, OrgChartAPIService, VRModalService, VRNotificationService, UtilsService, WhS_BE_MainService) {

    var users = [];
    var members = [];
    var assignedOrgChartId = undefined;
    var gridAPI;
    var treeAPI;

    defineScope();
    load();

    function defineScope() {
        $scope.nodes = [];
        $scope.openOrgChartsModal = openOrgChartsModal;
        $scope.assignCarriers = assignCarriers;
        $scope.onGridReady = function (api) {
            gridAPI = api;
            var filter = {};
            api.loadGrid(filter);
        }

        $scope.treeReady = function (api) {
            treeAPI = api;
        }

        $scope.treeValueChanged = function () {
            if (angular.isObject($scope.currentNode) && $scope.currentNode != undefined) {
                gridAPI.loadGrid(getFilterData());
            }
        }
    }

    function load() {
        $scope.isLoading = true;

        UtilsService.waitMultipleAsyncOperations([getUsers, getLinkedOrgChartId])
        .then(function () {
            if (assignedOrgChartId != undefined) {
                buildTreeFromOrgHierarchy();
            }
            else {
                buildTreeFromUsers();
            }
        })
        .catch(function (error) {
            $scope.isLoading = false;
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
    }

    function getFilterData() {
        if ($scope.currentNode != undefined) {
            var query = {
                ManagerId: $scope.currentNode.nodeId,
                WithDescendants: (assignedOrgChartId != undefined)
            };

            return query;
        }
    }

    function getUsers() {
        return UsersAPIService.GetUsers()
            .then(function (response) {
                users = response;
            });
    }

    function getLinkedOrgChartId() {
        return AccountManagerAPIService.GetLinkedOrgChartId()
            .then(function (response) {
                assignedOrgChartId = response;
            });
    }

    function buildTreeFromOrgHierarchy() {
        return OrgChartAPIService.GetOrgChartById(assignedOrgChartId)
            .then(function (orgChartObject) {
                members = orgChartObject.Hierarchy;
                addMissingMembers();
                $scope.nodes = mapMembersToNodes(members);
                treeAPI.refreshTree($scope.nodes);
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isLoading = false;
            });
    }
    
    function addMissingMembers() {
        for (var i = 0; i < users.length; i++) {
            var member = findNode(users[i].UserId);

            if (member == null) { // the user isn't a member
                var object = {
                    Id: users[i].UserId,
                    Name: users[i].Name,
                    Members: []
                };

                members.push(object);
            }
        }
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

    function buildTreeFromUsers() {
        $scope.nodes = [];

        for (var i = 0; i < users.length; i++) {
            var node = mapUserToNode(users[i]);
            $scope.nodes.push(node);
        }

        treeAPI.refreshTree($scope.nodes);
        $scope.isLoading = false;
    }

    function mapUserToNode(user) {
        return {
            nodeId: user.UserId,
            nodeName: user.Name,
            nodeChildren: [],
            isOpened: true
        };
    }

    function mapMembersToNodes(members) {
        if (members.length == 0)
            return [];

        var temp = [];

        for (var i = 0; i < members.length; i++) {
            var obj = {
                nodeId: members[i].Id,
                nodeName: UtilsService.getItemByVal(users, members[i].Id, 'UserId').Name,
                nodeChildren: mapMembersToNodes(members[i].Members),
                isOpened: true
            };

            temp.push(obj);
        }

        return temp;
    }

   

    function openOrgChartsModal() {
        var onOrgChartAssigned = function (orgChartId) {
            assignedOrgChartId = orgChartId;
            buildTreeFromOrgHierarchy();
            $scope.currentNode = undefined;
            var filter = {};
            gridAPI.loadGrid(filter);
        };

        WhS_BE_MainService.openOrgChartsModal(onOrgChartAssigned, assignedOrgChartId);
    }

    function assignCarriers() {
            var onCarriersAssigned = function () {
                gridAPI.loadGrid(getFilterData());
            }
            WhS_BE_MainService.assignCarriers(onCarriersAssigned, $scope.currentNode.nodeId);
    }

}

appControllers.controller('WhS_BE_AccountManagerManagementController', AccountManagerManagementController);
