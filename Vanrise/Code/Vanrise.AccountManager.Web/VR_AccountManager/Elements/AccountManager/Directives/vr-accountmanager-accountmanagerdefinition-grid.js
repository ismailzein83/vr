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
        function initializeController()
        {
            $scope.assignmentDefinitions = [];
            $scope.addNewAssignmentDefinition = function () {
                var onAssignmentDefinitionAdded = function (assignmnetDefifniton) {
                    $scope.assignmentDefinitions.push({Entity: assignmnetDefifniton});
                };
                VR_AccountManager_AccountManagerService.addAssignmentDefinition(onAssignmentDefinitionAdded);
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
                            if (payload.assignmentdefinitons != undefined)
                            {
                                for(var i= 0;i< payload.assignmentdefinitons.length;i++)
                                {
                                    var assignmentdefiniton = payload.assignmentdefinitons[i];
                                    $scope.assignmentDefinitions.push({ Entity: assignmentdefiniton });
                                }
                            }
                        }
                    };
                    directiveAPI.getData = function () {
                        var assignmentDefinitions = [];
                        if ($scope.assignmentDefinitions != undefined)
                        {
                            for (var i = 0; i < $scope.assignmentDefinitions.length; i++) {
                                var assignmentDefinition = $scope.assignmentDefinitions[i];
                                assignmentDefinitions.push(assignmentDefinition.Entity);
                            }
                        }
                        return assignmentDefinitions;
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
                var index = $scope.assignmentDefinitions.indexOf(assignmentDefinitionObj);
                $scope.assignmentDefinitions[index] = {Entity:assignmentDefinition};
            };
            VR_AccountManager_AccountManagerService.editAssignmentDefinition(assignmentDefinitionObj.Entity, onAssignmentDefinitionUpdated, $scope.assignmentDefinitions);
        }
    }
    return directiveDefinitionObject;
}]);