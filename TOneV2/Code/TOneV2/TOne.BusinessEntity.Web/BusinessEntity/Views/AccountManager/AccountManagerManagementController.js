AccountManagerManagementController.$inject = ['$scope', 'AccountManagerAPIService', 'UsersAPIService', 'ApplicationParameterAPIService', 'OrgChartAPIService', 'VRModalService', 'UtilsService'];

function AccountManagerManagementController($scope, AccountManagerAPIService, UsersAPIService, ApplicationParameterAPIService, OrgChartAPIService, VRModalService, UtilsService) {
    var gridApi;
    defineScope();
    load();

    function defineScope() {
        $scope.users = [];
        $scope.nodes = [];
        $scope.assignedOrgChart = undefined;
        $scope.assignedCarriers = [];

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
                orgChart: $scope.assignedOrgChart,
                accountManager: $scope.tree.currentNode
            };

            VRModalService.showModal('/Client/Modules/BusinessEntity/Views/AccountManager/CarrierAssignmentEditor.html', parameters, settings);
        }
        $scope.onGridReady = function (api) {
            gridApi = api;
        }
        $scope.$watch('tree.currentNode', function (newObj, oldObj) {
            if (angular.isObject($scope.tree.currentNode)) {
                getData();
            }
        }, false);
    }

    function load() {
        UsersAPIService.GetUsers().then(function (data) {
            $scope.users = data; // mapMembersToNodes depends on $scope.users
            //mapUsersToNodes();

            ApplicationParameterAPIService.GetApplicationParameterById(1).then(function (data) {
                var orgChartId = data.Value;

                OrgChartAPIService.GetOrgChartById(orgChartId).then(function (orgChart) {
                    console.log(orgChart);
                    $scope.nodes = mapMembersToNodes(orgChart.Hierarchy);
                });
            });
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

    function getData() {
        $scope.assignedCarriers = [];
        //var memberIds = AccountManagerAPIService.GetMemberIds($scope.assignedOrgChart.Id, $scope.tree.currentNode.nodeId);
        //AccountManagerAPIService.GetCarriers(1, 1000).then(function (data) {

        //});
        console.log($scope.assignedOrgChart.Id);
        console.log($scope.tree.currentNode.nodeId);
        return AccountManagerAPIService.GetAssignedCarriers([$scope.assignedOrgChart.Id, $scope.tree.currentNode.nodeId]).then(function (data) {
            angular.forEach(data, function (item) {
                console.log(item);
                console.log('currentNode.nodeId: ' + $scope.tree.currentNode.nodeId);
                var object = {
                    CarrierAccountId: item.CarrierAccountId,
                    IsCustomer: (item.RelationType == 1) ? 'True' : 'False',
                    IsSupplier: (item.RelationType == 2) ? 'True' : 'False',
                    Relationship: (item.UserId == $scope.tree.currentNode.nodeId) ? 'Direct' : 'Indirect'
                };

                $scope.assignedCarriers.push(object);
            });
        });
    }
}

appControllers.controller('BusinessEntity_AccountManagerManagementController', AccountManagerManagementController);
