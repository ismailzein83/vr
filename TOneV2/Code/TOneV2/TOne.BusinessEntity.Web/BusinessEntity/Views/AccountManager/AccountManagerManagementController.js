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
                WithDescendants: (assignedOrgChartId != undefined),
                CarrierType: 0
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
        var mergedCarriers = getMergedAssignedCarriers(assignedCarriers);

        var mappedCarriers = [];

        angular.forEach(mergedCarriers, function (item) {

            var gridObject = {
                CarrierAccountId: item.CarrierAccountId,
                CarrierName: item.CarrierName,
                IsCustomer: (item.RelationType != 2) ? 'True' : 'False',
                IsSupplier: (item.RelationType != 1) ? 'True' : 'False',
                Access: (item.UserId == $scope.currentNode.nodeId) ? 'Direct' : 'Indirect'
            };

            mappedCarriers.push(gridObject);
        });
        
        return mappedCarriers;
    }

    function getMergedAssignedCarriers(carriers) {
        var mergedCarriers = [];

        angular.forEach(carriers, function (item) {
            if (countAssignedCarrier(mergedCarriers, item) == 0) // i.e. mergedCarriers doesn't contain item
                mergedCarriers.push(getMergedAssignedCarrier(carriers, item));
        });

        return mergedCarriers;
    }

    function countAssignedCarrier(carriers, carrier) {
        var counter = 0;

        angular.forEach(carriers, function (item) {
            if (item.UserId == carrier.UserId && item.CarrierAccountId == carrier.CarrierAccountId)
                counter++;
        });

        return counter;
    }

    function getMergedAssignedCarrier(carriers, carrier)
    {
        if (countAssignedCarrier(carriers, carrier) == 2)
            carrier.RelationType = 0; // indicating that this carrier is assigned as a customer and a supplier to this user

        return carrier; // the merged carrier
    }
}

appControllers.controller('BusinessEntity_AccountManagerManagementController', AccountManagerManagementController);
