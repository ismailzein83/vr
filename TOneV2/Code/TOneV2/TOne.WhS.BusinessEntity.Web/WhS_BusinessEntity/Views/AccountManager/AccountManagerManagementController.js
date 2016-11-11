(function (appControllers) {

    'use strict';

    AccountManagerManagementController.$inject = ['$scope', 'WhS_BE_AccountManagerAPIService', 'WhS_BE_AccountManagerService', 'VR_Sec_UserAPIService', 'VR_Sec_OrgChartAPIService', 'UtilsService', 'VRNotificationService'];

    function AccountManagerManagementController($scope, WhS_BE_AccountManagerAPIService, WhS_BE_AccountManagerService, VR_Sec_UserAPIService, VR_Sec_OrgChartAPIService, UtilsService, VRNotificationService) {
        var gridAPI;
        var treeAPI;
        var treeReadyDeferred = UtilsService.createPromiseDeferred();

        var nodes;
        var members;

        var users;
        var assignedOrgChartId;

        defineScope();
        load();

        function defineScope() {

            $scope.hasUpdateLinkedOrgChartPermission = function () {
                return WhS_BE_AccountManagerAPIService.HasUpdateLinkedOrgChartPermission();
            };

            $scope.hasAssignCarriersPermission = function () {
                return WhS_BE_AccountManagerAPIService.HasAssignCarriersPermission();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(getFilterObject());
            };

            $scope.onTreeReady = function (api) {
                treeAPI = api;
                treeReadyDeferred.resolve();
            };

            $scope.onTreeValueChanged = function () {
                if (angular.isObject($scope.currentNode) && $scope.currentNode != undefined) {
                    gridAPI.loadGrid(getFilterObject());
                }
            };

            $scope.assignOrgChart = function () {
                var onOrgChartAssigned = function (orgChartId) {
                    assignedOrgChartId = orgChartId;
                    buildTreeFromOrgHierarchy(); // In this case, the grid could be loaded without waiting for the tree to load because currentNode = undefined
                    $scope.currentNode = undefined;
                    gridAPI.loadGrid(getFilterObject());
                };
                WhS_BE_AccountManagerService.assignOrgChart(assignedOrgChartId, onOrgChartAssigned);
            };

            $scope.assignCarriers = function () {
                var onCarriersAssigned = function () {
                    gridAPI.loadGrid(getFilterObject());
                };
                WhS_BE_AccountManagerService.assignCarriers($scope.currentNode.nodeId, onCarriersAssigned);
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function getFilterObject() {
            return $scope.currentNode ? {
                ManagerId: $scope.currentNode.nodeId,
                WithDescendants: assignedOrgChartId != undefined
            } : {};
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadTree]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function loadTree() {
            var loadTreeDeferred = UtilsService.createPromiseDeferred();

            UtilsService.waitMultipleAsyncOperations([loadUsers, getLinkedOrgChartId]).then(function () {
                if (assignedOrgChartId) {
                    buildTreeFromOrgHierarchy().then(function () {
                        loadTreeDeferred.resolve();
                    }).catch(function (error) { loadTreeDeferred.reject(error); });
                }
                else {
                    buildTreeFromUsers().then(function () {
                        loadTreeDeferred.resolve();
                    }).catch(function (error) { loadTreeDeferred.reject(error); });
                }
            }).catch(function (error) {
                loadTreeDeferred.reject(error);
            });

            return loadTreeDeferred.promise;

            function loadUsers() {
                return VR_Sec_UserAPIService.GetUsers().then(function (response) {
                    if (response) {
                        users = [];

                        for (var i = 0; i < response.length; i++) {
                            users.push(response[i]);
                        }
                    }
                });
            }

            function getLinkedOrgChartId() {
                return WhS_BE_AccountManagerAPIService.GetLinkedOrgChartId().then(function (response) {
                    assignedOrgChartId = response;
                });
            }
        }

        function buildTreeFromOrgHierarchy() {
            var loadHierarchyDeferred = UtilsService.createPromiseDeferred();

            VR_Sec_OrgChartAPIService.GetOrgChartById(assignedOrgChartId).then(function (orgChartObject) {
                members = orgChartObject.Hierarchy;
                addMissingMembers();
                nodes = mapMembersToNodes(members);

                treeReadyDeferred.promise.then(function () {
                    loadHierarchyDeferred.resolve();
                    treeAPI.refreshTree(nodes);
                });
            }).catch(function (error) {
                loadHierarchyDeferred.reject(error);
            });

            return loadHierarchyDeferred.promise;
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
            var loadHierarchyDeferred = UtilsService.createPromiseDeferred();
            nodes = [];

            for (var i = 0; i < users.length; i++) {
                var node = mapUserToNode(users[i]);
                nodes.push(node);
            }

            treeReadyDeferred.promise.then(function () {
                loadHierarchyDeferred.resolve();
                treeAPI.refreshTree(nodes);
            });

            return loadHierarchyDeferred.promise;
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
                var user = UtilsService.getItemByVal(users, members[i].Id, 'UserId');
                var obj = {
                    nodeId: members[i].Id,
                    nodeName: user != undefined ? user.Name : undefined,
                    nodeChildren: mapMembersToNodes(members[i].Members),
                    isOpened: true
                };

                temp.push(obj);
            }

            return temp;
        }
    }

    appControllers.controller('WhS_BE_AccountManagerManagementController', AccountManagerManagementController);

})(appControllers);
