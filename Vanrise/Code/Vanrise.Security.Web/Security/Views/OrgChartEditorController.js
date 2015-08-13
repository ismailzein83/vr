OrgChartEditorController.$inject = ['$scope', 'OrgChartAPIService', 'UsersAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function OrgChartEditorController($scope, OrgChartAPIService, UsersAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {

    var orgChartId = undefined;
    var users = [];
    var members = [];

    var editMode;
    var treeAPI;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        orgChartId = undefined;

        if (parameters != undefined && parameters != null) {
            orgChartId = parameters.orgChartId;
        }

        editMode = (orgChartId != undefined);
    }

    function defineScope(){

        $scope.saveOrgChart = function () {
            if (editMode)
                return updateOrgChart();
            else
                return insertOrgChart();
        };

        $scope.close = function () {
            $scope.modalContext.closeModal();
        };

        $scope.treeReady = function (api) {
            treeAPI = api;

            //if (members.length > 0)
            //    treeAPI.refreshTree(members);
        }
    }

    function load() {
        $scope.isGettingData = true;

        UsersAPIService.GetUsers()
            .then(function (response) {
                angular.forEach(response, function (item) {
                    users.push(item);
                });

                if (editMode)
                    getOrgChart(); // uses users at some point
                else {
                    mapUsersToMembers();
                    $scope.isGettingData = false;
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            });
    }

    function getOrgChart() {

        return OrgChartAPIService.GetOrgChartById(orgChartId)
            .then(function (response) {
                fillScopeFromOrgChartObj(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isGettingData = false;
            });
    }

    function mapUsersToMembers() {
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

    function fillScopeFromOrgChartObj(orgChartObject) {
        $scope.name = orgChartObject.Name;
        addMappedUnassignedUsersToHierarchy(orgChartObject.Hierarchy);

        members = mapHierarchyToTreeNodes(orgChartObject.Hierarchy);
        treeAPI.refreshTree(members);
    }

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

    function findNode(hierarchy, id) {
        for (var i = 0; i < hierarchy.length; i++) {
            var node = getNodeRecursively(id, hierarchy[i]);
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

    function insertOrgChart() {
        var orgChartObject = buildOrgChartObjFromScope();

        return OrgChartAPIService.AddOrgChart(orgChartObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Org Chart", response)) {
                    if ($scope.onOrgChartAdded != undefined)
                        $scope.onOrgChartAdded(response.InsertedObject);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function updateOrgChart() {
        var orgChartObject = buildOrgChartObjFromScope();

        return OrgChartAPIService.UpdateOrgChart(orgChartObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Org Chart", response)) {
                    if ($scope.onOrgChartUpdated != undefined)
                        $scope.onOrgChartUpdated(response.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function buildOrgChartObjFromScope() {
        return {
            Id: (orgChartId != null) ? orgChartId : 0,
            Name: $scope.name,
            Hierarchy: (treeAPI.getTree != undefined) ? treeAPI.getTree() : members
        };
    }
}

appControllers.controller('Security_OrgChartEditorController', OrgChartEditorController);