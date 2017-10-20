"use strict";

app.directive("vrAccountmanagerAccountmanagerdefinitionGrid", ["UtilsService", "VRNotificationService", "VRUIUtilsService","VR_AccountManager_AccountManagerService",
function (UtilsService, VRNotificationService, VRUIUtilsService, VR_AccountManager_AccountManagerService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountManagerDefinitionGrid = new AccountManagerDefinitionGrid($scope, ctrl, $attrs);
            accountManagerDefinitionGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/VR_AccountManager/Elements/AccountManager/Directives/Template/AccountManagerDefinitionGridTemplate.html'
    };

    function AccountManagerDefinitionGrid($scope, ctrl, $attrs)
    {
        this.initializeController = initializeController;
        var gridAPI;
        $scope.accountManagerDefinitions = [];
        function initializeController()
        {
            $scope.addNewAssignmentDefinition = function () {
                var onAssignmentDefinitionAdded = function (assignmnetDefifniton) {
                    $scope.accountManagerDefinitions.push(assignmnetDefifniton);
                };
                VR_AccountManager_AccountManagerService.addNewAssignmentDefinition(onAssignmentDefinitionAdded);
            };
           
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveAPI());
                }
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (payload) {
                        if (payload != undefined) {
                            $scope.accountManagerDefinitions = payload;
                        }
                    };
                    directiveAPI.getData = function () {
                        return ($scope.accountManagerDefinitions);
                    };
                    return directiveAPI;
                };

            };
            defineMenuActions();
        }
        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editAssignmentDefinition,
            }];
        }
        function editAssignmentDefinition(assignmentDefinitionObj) {
            var onAssignmentDefinitionUpdated = function (assignmentDefinition) {
                var index =$scope.accountManagerDefinitions.indexOf(assignmentDefinitionObj);
                $scope.accountManagerDefinitions[index] = assignmentDefinition;
            };
            VR_AccountManager_AccountManagerService.editAssignmentDefinition(assignmentDefinitionObj, onAssignmentDefinitionUpdated, $scope.accountManagerDefinitions);
        }
    }
    return directiveDefinitionObject;
}]);