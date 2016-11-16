"use strict";

app.directive("vrSecGroupGrid", ["VR_Sec_GroupAPIService", "VR_Sec_GroupService", "VR_Sec_PermissionService", "VR_Sec_HolderTypeEnum", 'VRNotificationService', 'VR_Sec_PermissionAPIService', function (VR_Sec_GroupAPIService, VR_Sec_GroupService, VR_Sec_PermissionService, VR_Sec_HolderTypeEnum, VRNotificationService, VR_Sec_PermissionAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new GroupGridCtor($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Security/Directives/Group/Templates/GroupGrid.html"

    };

    function GroupGridCtor($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.groups = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };

                    directiveAPI.onGroupAdded = function (groupObject) {
                        gridAPI.itemAdded(groupObject);
                    };

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_Sec_GroupAPIService.GetFilteredGroups(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                    name: "Edit",
                    clicked: editGroup,
                    //permissions: "Root/Administration Module/Groups:Edit"
                    haspermission: hasEditGroupPermission
                },
                {
                    name: "Assign Permissions",
                    clicked: assignPermissions,
                    //permissions: "Root/Administration Module/System Entities:Assign Permissions"
                    haspermission: hasEditPermissionPermission
                }
            ];
        }

        function hasEditGroupPermission() {
            return VR_Sec_GroupAPIService.HasEditGroupPermission();
        }

        function hasEditPermissionPermission() {
            return VR_Sec_PermissionAPIService.HasUpdatePermissionsPermission();
        }

        function editGroup(groupObj) {
            var onGroupUpdated = function (groupObj) {
                gridAPI.itemUpdated(groupObj);
            };

            VR_Sec_GroupService.editGroup(groupObj.Entity.GroupId, onGroupUpdated);
        }

        function assignPermissions(groupObj) {
            VR_Sec_PermissionService.assignPermissions(VR_Sec_HolderTypeEnum.Group.value, groupObj.Entity.GroupId);
        }
    }

    return directiveDefinitionObject;

}]);