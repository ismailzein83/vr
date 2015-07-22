AccountManagerManagementController.$inject = ['$scope', 'AccountManagerAPIService', 'UsersAPIService', 'ApplicationParameterAPIService', 'OrgChartAPIService', 'VRModalService', 'UtilsService'];

function AccountManagerManagementController($scope, AccountManagerAPIService, UsersAPIService, ApplicationParameterAPIService, OrgChartAPIService, VRModalService, UtilsService) {
    var gridApi;
    var users = [];
    var assignedOrgChart = undefined;
    defineScope();
    load();

    function defineScope() {
        $scope.nodes = [];
        $scope.assignedCarriers = [];

        $scope.openOrgChartsModal = function () {
            var settings = {};
            var parameters = null;

            if (assignedOrgChart != undefined) {
                parameters = {
                    assignedOrgChart: assignedOrgChart
                };
            }

            settings.onScopeReady = function (modalScope) {
                modalScope.title = 'Assign Org Chart';
                modalScope.onOrgChartAssigned = function (orgChart) {
                    $scope.nodes = mapMembersToNodes(orgChart.Hierarchy);
                    assignedOrgChart = orgChart;
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
                orgChart: assignedOrgChart,
                selectedAccountManagerId: $scope.tree.currentNode.nodeId
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
        UsersAPIService.GetUsers().then(function (response) {
            users = response; // mapMembersToNodes depends on users
            //mapUsersToNodes();

            ApplicationParameterAPIService.GetApplicationParameterById(1).then(function (response) {
                var orgChartId = response.Value;

                OrgChartAPIService.GetOrgChartById(orgChartId).then(function (orgChart) {
                    $scope.nodes = mapMembersToNodes(orgChart.Hierarchy);
                });
            });
        });

        AccountManagerAPIService.GetLinkedOrgChartId().then(function (response) {
            console.log(response);
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

    function mapUsersToNodes() {
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
        $scope.assignedCarriers = [];

        return AccountManagerAPIService.GetAssignedCarriers([assignedOrgChart.Id, $scope.tree.currentNode.nodeId]).then(function (response) {
            angular.forEach(response, function (item) {
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
