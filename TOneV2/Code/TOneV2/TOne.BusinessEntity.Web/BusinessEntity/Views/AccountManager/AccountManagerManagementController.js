AccountManagerManagementController.$inject = ['$scope', 'AccountManagerAPIService', 'UsersAPIService', 'OrgChartAPIService', 'ApplicationParameterAPIService', 'OrgChartAPIService', 'VRModalService', 'UtilsService'];

function AccountManagerManagementController($scope, AccountManagerAPIService, UsersAPIService, OrgChartAPIService, ApplicationParameterAPIService, OrgChartAPIService, VRModalService, UtilsService) {
    var gridApi;
    var users = [];
    var assignedOrgChartId = undefined;
    var treeAPI;
    defineScope();
    load();

    function defineScope() {
        $scope.nodes = [];
        $scope.assignedCarriers = [];

        $scope.openOrgChartsModal = function () {
            var settings = {};
            var parameters = null;

            if (assignedOrgChartId != 0) {
                parameters = {
                    assignedOrgChartId: assignedOrgChartId
                };
            }

            settings.onScopeReady = function (modalScope) {
                modalScope.title = 'Assign Org Chart';

                modalScope.onOrgChartAssigned = function (orgChartId) {
                    buildNodesFromMembers(orgChartId).then(function () {
                        assignedOrgChartId = orgChartId;
                        $scope.currentNode = undefined; // deselect the account manager
                        treeAPI.refreshTree($scope.nodes);
                    });
                }
            };

            VRModalService.showModal('/Client/Modules/BusinessEntity/Views/AccountManager/OrgChartAssignmentEditor.html', parameters, settings);
        }

        $scope.openCarriersModal = function () {
            var settings = {};

            var parameters = {
                selectedAccountManagerId: $scope.currentNode.nodeId
            };
            
            settings.onScopeReady = function (modalScope) {
                modalScope.title = 'Assign Carriers';
                modalScope.onCarriersAssigned = function () {
                    getData();
                }
            };

            VRModalService.showModal('/Client/Modules/BusinessEntity/Views/AccountManager/CarrierAssignmentEditor.html', parameters, settings);
        }

        $scope.onGridReady = function (api) {
            gridApi = api;
        }

        $scope.treeReady = function (api) {
            treeAPI = api;
        }

        $scope.treeValueChanged = function () {
            if (angular.isObject($scope.currentNode) && $scope.currentNode != undefined) {
                // clear the grid
                $scope.assignedCarriers = [];
                getData();
            }
        }
    }

    function load() {
        UsersAPIService.GetUsers().then(function (response) { // make sure that users is set
            users = response; // mapMembersToNodes depends on users

            AccountManagerAPIService.GetLinkedOrgChartId().then(function (response) {
                if (response == undefined || response == null) {
                    buildNodesFromUsers();
                    treeAPI.refreshTree($scope.nodes);
                }
                else {
                    OrgChartAPIService.GetOrgChartById(response).then(function (globalOrgChart) {
                        $scope.nodes = mapMembersToNodes(globalOrgChart.Hierarchy);
                        treeAPI.refreshTree($scope.nodes);
                        assignedOrgChartId = globalOrgChart.Id;
                    });
                }
            });
        });
    }

    function buildNodesFromMembers(orgChartId) {
       return OrgChartAPIService.GetOrgChartById(orgChartId).then(function (orgChartObject) {
            $scope.nodes = mapMembersToNodes(orgChartObject.Hierarchy);
        });
    }

    function mapMembersToNodes(members) {
        if (members.length == 0)
            return [];

        var temp = [];

        for (var i = 0; i < members.length; i++) {
            var obj = {
                nodeId: members[i].Id,
                nodeName: UtilsService.getItemByVal(users, members[i].Id, 'UserId').Name,
                nodeChildren: mapMembersToNodes(members[i].Members)
            };

            temp.push(obj);
        }

        return temp;
    }

    function buildNodesFromUsers() {
        $scope.nodes = [];

        for (var i = 0; i < users.length; i++) {
            var node = mapUserToNode(users[i]);
            $scope.nodes.push(node);
        }
    }

    function mapUserToNode(user) {
        return {
            nodeId: user.UserId,
            nodeName: user.Name,
            nodeChildren: []
        };
    }

    function getData() {

        if (assignedOrgChartId != undefined)
        {
            return AccountManagerAPIService.GetAssignedCarriers($scope.currentNode.nodeId, true).then(function (response) {
                fillAssignedCarriers(response);
            });
        }
        else
        {
            return AccountManagerAPIService.GetAssignedCarriers($scope.currentNode.nodeId, false).then(function (response) {
                fillAssignedCarriers(response);
            });
        }
    }

    function fillAssignedCarriers(assignedCarriers)
    {
        angular.forEach(assignedCarriers, function (item) {
            var object = {
                CarrierName: item.CarrierName,
                CarrierAccountId: item.CarrierAccountId,
                IsCustomer: (item.RelationType == 1) ? 'True' : 'False',
                IsSupplier: (item.RelationType == 2) ? 'True' : 'False',
                Access: (item.UserId == $scope.currentNode.nodeId) ? 'Direct' : 'Indirect'
            };

            $scope.assignedCarriers.push(object);
        });
    }
}

appControllers.controller('BusinessEntity_AccountManagerManagementController', AccountManagerManagementController);
