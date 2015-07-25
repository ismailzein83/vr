AccountManagerManagementController.$inject = ['$scope', 'AccountManagerAPIService', 'UsersAPIService', 'OrgChartAPIService', 'ApplicationParameterAPIService', 'OrgChartAPIService', 'VRModalService', 'VRNotificationService', 'UtilsService'];

function AccountManagerManagementController($scope, AccountManagerAPIService, UsersAPIService, OrgChartAPIService, ApplicationParameterAPIService, OrgChartAPIService, VRModalService, VRNotificationService, UtilsService) {

    var users = [];
    var members = [];
    var assignedOrgChartId = undefined;
    var gridApi;
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
                    assignedOrgChartId = orgChartId;

                    getMembers().finally(function () {
                        buildNodesFromMembers();
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
                    // clear the grid
                    $scope.assignedCarriers = [];

                    $scope.isGettingData = true;
                    getData().finally(function () {
                        $scope.isGettingData = false;
                    });
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
            $scope.isGettingData = true;

            if (angular.isObject($scope.currentNode) && $scope.currentNode != undefined) {
                // clear the grid
                $scope.assignedCarriers = [];

                getData().finally(function () {
                    $scope.isGettingData = false;
                });
            }
        }
    }

    function load() {

        getUsers().finally(function () {

            getLinkedOrgChartId().finally(function () {

                if (assignedOrgChartId == undefined || assignedOrgChartId == null) {
                    buildNodesFromUsers();
                    treeAPI.refreshTree($scope.nodes);
                }
                else {
                    getMembers().finally(function () {
                        buildNodesFromMembers(); // requires users to be set
                        treeAPI.refreshTree($scope.nodes);
                    });
                }
            });
        });
    }

    function getUsers() {
        return UsersAPIService.GetUsers()
            .then(function (response) {
                users = response;
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }

    function getLinkedOrgChartId() {
        return AccountManagerAPIService.GetLinkedOrgChartId()
            .then(function (response) {
                assignedOrgChartId = response;
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }

    function getMembers() {
        return OrgChartAPIService.GetOrgChartById(assignedOrgChartId)
            .then(function (orgChartObject) {
                members = orgChartObject.Hierarchy;
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
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
            nodeChildren: [],
            isOpened: true
        };
    }

    function buildNodesFromMembers() {
        $scope.nodes = mapMembersToNodes(members);
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

    function getData() {

        if (assignedOrgChartId != undefined)
        {
            return AccountManagerAPIService.GetAssignedCarriers($scope.currentNode.nodeId, true)
                .then(function (response) {
                    fillAssignedCarriers(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }
        else
        {
            return AccountManagerAPIService.GetAssignedCarriers($scope.currentNode.nodeId, false)
                .then(function (response) {
                    fillAssignedCarriers(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }
    }

    function fillAssignedCarriers(assignedCarriers)
    {
        angular.forEach(assignedCarriers, function (item) {
            var gridObject = {
                CarrierName: item.CarrierName,
                CarrierAccountId: item.CarrierAccountId,
                IsCustomer: (item.RelationType == 1) ? 'True' : 'False',
                IsSupplier: (item.RelationType == 2) ? 'True' : 'False',
                Access: (item.UserId == $scope.currentNode.nodeId) ? 'Direct' : 'Indirect'
            };

            $scope.assignedCarriers.push(gridObject);
        });
    }
}

appControllers.controller('BusinessEntity_AccountManagerManagementController', AccountManagerManagementController);
