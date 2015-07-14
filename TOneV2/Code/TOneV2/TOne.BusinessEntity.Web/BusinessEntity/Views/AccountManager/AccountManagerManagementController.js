AccountManagerManagementController.$inject = ['$scope', 'UsersAPIService', 'VRModalService', 'UtilsService'];

function AccountManagerManagementController($scope, UsersAPIService, VRModalService, UtilsService) {
    defineScope();
    load();

    function defineScope() {
        $scope.users = [];
        $scope.nodes = [];
        $scope.assignedOrgChart = undefined;

        $scope.openOrgChartsModal = function () {
            var settings = {};
            var parameters = null;

            if ($scope.assignedOrgChart != undefined) {
                parameters = {
                    assignedOrgChart: $scope.assignedOrgChart
                };
            }

            settings.onScopeReady = function (modalScope) {
                modalScope.title = 'Assign Org Chart';
                modalScope.onOrgChartAssigned = function (orgChart) {
                    $scope.nodes = mapMembersToNodes(orgChart.Hierarchy);
                    $scope.assignedOrgChart = orgChart;
                    $scope.tree.currentNode = undefined; // deselect the account manager
                }
            };

            VRModalService.showModal('/Client/Modules/BusinessEntity/Views/AccountManager/OrgChartAssignmentEditor.html', parameters, settings);
        }
        $scope.openCarriersModal = function () {
            var settings = {};
            
            settings.onScopeReady = function (modalScope) {
                modalScope.title = 'Assign Carriers';
            };

            var parameters = {
                accountManager: $scope.tree.currentNode
            };

            VRModalService.showModal('/Client/Modules/BusinessEntity/Views/AccountManager/CarrierAssignmentEditor.html', parameters, settings);
        }
    }

    function load() {
        UsersAPIService.GetUsers().then(function (data) {
            $scope.users = data;
            mapUsersToNodes();
        });
    }

    function mapMembersToNodes(members) {
        if (members.length == 0)
            return [];

        var temp = [];

        for (var i = 0; i < members.length; i++) {
            var obj = {
                nodeId: members[i].Id,
                nodeName: UtilsService.getItemByVal($scope.users, members[i].Id, 'UserId').Name,
                nodeChildren: mapMembersToNodes(members[i].Members)
            };

            temp.push(obj);
        }

        return temp;
    }

    function mapUsersToNodes() {
        for (var i = 0; i < $scope.users.length; i++) {
            var node = mapUserToNode($scope.users[i]);
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
}

appControllers.controller('BusinessEntity_AccountManagerManagementController', AccountManagerManagementController);
