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

                    buildTreeFromOrgHierarchy();
                    $scope.currentNode = undefined;
                    return retrieveData();
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
                    return retrieveData();
                }
            };

            VRModalService.showModal('/Client/Modules/BusinessEntity/Views/AccountManager/CarrierAssignmentEditor.html', parameters, settings);
        }

        $scope.gridReady = function (api) {
            gridApi = api;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            
            return AccountManagerAPIService.GetAssignedCarriersFromTempTable(dataRetrievalInput)
                .then(function (response) {

                    response.Data = getMappedAssignedCarriers(response.Data);
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }

        $scope.treeReady = function (api) {
            treeAPI = api;
        }

        $scope.treeValueChanged = function () {
            if (angular.isObject($scope.currentNode) && $scope.currentNode != undefined) {
                return retrieveData();
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

    function retrieveData() {
        if ($scope.currentNode != undefined) {
            var query = {
                ManagerId: $scope.currentNode.nodeId,
                WithDescendants: (assignedOrgChartId != undefined)
            };

            return gridApi.retrieveData(query);
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

    function getMappedAssignedCarriers(assignedCarriers)
    {
        var mappedCarriers = [];

        angular.forEach(assignedCarriers, function (item) {

            var gridObject = {
                CarrierAccountID: item.CarrierAccountID,
                CarrierName: item.CarrierName,
                IsCustomerAssigned: (item.IsCustomerIndirect) ? (item.IsCustomerAssigned + ' (Indirect)') : item.IsCustomerAssigned,
                IsSupplierAssigned: (item.IsSupplierIndirect) ? (item.IsSupplierAssigned + ' (Indirect)') : item.IsSupplierAssigned,
                IsCustomerIndirect: item.IsCustomerIndirect,
                IsSupplierIndirect: item.IsSupplierIndirect
            };

            mappedCarriers.push(gridObject);
        });
        
        return mappedCarriers;
    }
}

appControllers.controller('BusinessEntity_AccountManagerManagementController', AccountManagerManagementController);
