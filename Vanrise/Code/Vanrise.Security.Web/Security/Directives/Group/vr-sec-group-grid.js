"use strict";

app.directive("vrSecUserGrid", ["VRNotificationService", "VR_Sec_GroupAPIService", "VR_Sec_GroupService",
function (VRNotificationService, VR_Sec_GroupAPIService, VR_Sec_GroupService) {

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
        templateUrl: "/Client/Modules/Security/Directives/User/Templates/GroupGrid.html"

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
                    }

                    directiveAPI.onUserAdded = function (groupObject) {
                        gridAPI.itemAdded(groupObject);
                    }

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
                    permissions: "Root/Administration Module/Groups:Edit"
                },
                {
                    name: "Assign Permissions",
                    clicked: assignPermissions,
                    permissions: "Root/Administration Module/System Entities:Assign Permissions"
                }
            ];
        }

        function editGroup(groupObj) {
            var onGroupUpdated = function (groupObj) {
                gridAPI.itemUpdated(groupObj);
            }

            VR_Sec_GroupService.editUser(groupObj.Entity.UserId, onGroupUpdated);
        }

        function resetPassword(userObj) {
            VR_Sec_UserService.resetPassword(userObj.Entity.UserId);
        }

        function assignPermissions(userObj) {
            VR_Sec_UserService.assignPermissions(userObj.Entity.UserId);
        }
    }

    return directiveDefinitionObject;

}]);