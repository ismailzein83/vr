(function (appControllers) {

    'use strict';

    OrgChartEditorController.$inject = ['$scope', 'VR_Sec_OrgChartAPIService', 'VR_Sec_UserAPIService', 'UtilsService', 'VRNavigationService', 'VRNotificationService'];

    function OrgChartEditorController($scope, VR_Sec_OrgChartAPIService, VR_Sec_UserAPIService, UtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;

        var orgChartId;
        var orgChartEntity;

        var users;
        var members;

        var treeAPI;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                orgChartId = parameters.orgChartId;
            }

            isEditMode = (orgChartId != undefined);
        }

        function defineScope() {
            $scope.modalScope = {};

            $scope.modalScope.onTreeReady = function (api) {
                treeAPI = api;
            };

            $scope.modalScope.save = function () {
                if (isEditMode)
                    return updateOrgChart();
                else
                    return insertOrgChart();
            };

            $scope.modalScope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.modalScope.isLoading = true;

            if (isEditMode) {
                getOrgChart().then(function () {
                    loadAllControls().finally(function () {
                        orgChartEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.modalScope.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope.modalScope);
                });
            }
            else {
                loadAllControls();
            }
        }

        function getOrgChart() {
            return VR_Sec_OrgChartAPIService.GetOrgChartById(orgChartId).then(function (response) {
                orgChartEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticControls, loadUsers]).then(function () {
                loadMembers();
            }).finally(function () {
                $scope.modalScope.isLoading = false;
            });

            function setTitle() {
                $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(orgChartEntity ? orgChartEntity.Name : null, "Org Chart") : UtilsService.buildTitleForAddEditor("Org Chart");
            }
            function loadStaticControls() {
                if (orgChartEntity) {
                    $scope.modalScope.name = orgChartEntity.Name;
                }
            }
            function loadUsers() {
                return VR_Sec_UserAPIService.GetUsersInfo().then(function (response) {
                    if (response) {
                        users = [];
                        for (var i = 0; i < response.length; i++) {
                            users.push(response[i]);
                        }
                    }
                });
            }
            function loadMembers() {
                if (isEditMode) {
                    loadMembersFromHierarchy();
                }
                else {
                    loadMembersFromUsers();
                }

                function loadMembersFromHierarchy() {
                    addMappedUnassignedUsersToHierarchy(orgChartEntity.Hierarchy);
                    removeDeletedUsersFromHierarchy(orgChartEntity.Hierarchy);

                    members = mapHierarchyToTreeNodes(orgChartEntity.Hierarchy);
                    treeAPI.refreshTree(members);

                    function addMappedUnassignedUsersToHierarchy(hierarchy) { // adds the unassigned users as members to the hierarchy
                        for (var i = 0; i < users.length; i++) {
                            var member = findNode(hierarchy, users[i].UserId);

                            if (member == undefined || member == null)
                                hierarchy.push({
                                    Id: users[i].UserId,
                                    Name: users[i].Name,
                                    Members: [],
                                    isOpened: true
                                });
                        }

                        function findNode(hierarchy, id) {
                            for (var i = 0; i < hierarchy.length; i++) {
                                var node = getNodeRecursively(id, hierarchy[i]);
                                if (node != null)
                                    return node;
                            }

                            return null;

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
                        }
                    }
                    function removeDeletedUsersFromHierarchy(mainHierarchy) {
                        //Deleted Users should be removed from the hierarchy
                        //Members under deleted users should be added to hierarchy's root
                        var objectsToAddToRoot = [];
                        var objectsIdsToDelete = [];
                        markObjectsToReindex(mainHierarchy, objectsToAddToRoot, objectsIdsToDelete);

                        angular.forEach(objectsIdsToDelete, function (id) {
                            //Remove the deleted users from the main tree
                            removeMemberById(mainHierarchy, id);
                            //Also clear the items to be added to root from deleted users
                            removeMemberById(objectsToAddToRoot, id);
                        });

                        angular.forEach(objectsToAddToRoot, function (item) {
                            mainHierarchy.push(item);
                        });

                        function markObjectsToReindex(mainHierarchy, objectsToAddToRoot, objectsIdsToDelete) {
                            for (var i = 0; i < mainHierarchy.length; i++) {
                                var userFound = UtilsService.getItemByVal(users, mainHierarchy[i].Id, 'UserId');

                                if (!userFound) {
                                    objectsIdsToDelete.push(mainHierarchy[i].Id);
                                    angular.forEach(mainHierarchy[i].Members, function (item) {
                                        objectsToAddToRoot.push(item);
                                    });
                                }

                                if (mainHierarchy[i].Members.length > 0)
                                    markObjectsToReindex(mainHierarchy[i].Members, objectsToAddToRoot, objectsIdsToDelete);
                            }
                        }
                        function removeMemberById(mainHierarchy, id) {
                            for (var i = 0; i < mainHierarchy.length; i++) {
                                if (mainHierarchy[i].Id == id) {
                                    mainHierarchy.splice(i, 1);
                                    return;
                                }
                                else if (mainHierarchy[i].Members.length > 0) {
                                    removeMemberById(mainHierarchy[i].Members, id);
                                }
                            }
                        }
                    }
                    function mapHierarchyToTreeNodes(array) { // returns an array of members that define the Name and isOpened attributes of a tree node
                        if (array.length == 0)
                            return [];

                        var temp = [];

                        for (var i = 0; i < array.length; i++) {
                            temp.push({
                                Id: array[i].Id,
                                Name: UtilsService.getItemByVal(users, array[i].Id, 'UserId').Name,
                                Members: mapHierarchyToTreeNodes(array[i].Members),
                                isOpened: true
                            });
                        }

                        return temp;
                    }
                }
                function loadMembersFromUsers() {
                    members = [];

                    for (var i = 0; i < users.length; i++) {
                        members.push({
                            Id: users[i].UserId,
                            Name: users[i].Name,
                            Members: [],
                            isOpened: true
                        });
                    }

                    treeAPI.refreshTree(members);
                };
            }
        }

        function insertOrgChart() {
            $scope.modalScope.isLoading = true;
            var orgChartObject = buildOrgChartObjFromScope();

            return VR_Sec_OrgChartAPIService.AddOrgChart(orgChartObject).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Org Chart', response, 'Name')) {
                    if ($scope.onOrgChartAdded && typeof $scope.onOrgChartAdded == 'function') {
                        $scope.onOrgChartAdded(response.InsertedObject);
                    }

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope.modalScope);
            }).finally(function () {
                $scope.modalScope.isLoading = false;
            });
        }

        function updateOrgChart() {
            $scope.modalScope.isLoading = true;
            var orgChartObject = buildOrgChartObjFromScope();

            return VR_Sec_OrgChartAPIService.UpdateOrgChart(orgChartObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Org Chart', response, 'Name')) {
                    if ($scope.onOrgChartUpdated && typeof $scope.onOrgChartUpdated == 'function') {
                        $scope.onOrgChartUpdated(response.UpdatedObject);
                    }   

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope.modalScope);
            }).finally(function () {
                $scope.modalScope.isLoading = false;
            });
        }

        function buildOrgChartObjFromScope() {
            return {
                OrgChartId: (orgChartId != null) ? orgChartId : 0,
                Name: $scope.modalScope.name,
                Hierarchy: (treeAPI.getTree != undefined) ? treeAPI.getTree() : members
            };
        }
    }

    appControllers.controller('VR_Sec_OrgChartEditorController', OrgChartEditorController);

})(appControllers);
