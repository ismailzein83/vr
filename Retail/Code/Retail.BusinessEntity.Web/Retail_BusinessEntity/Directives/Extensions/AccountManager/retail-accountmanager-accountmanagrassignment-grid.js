"use strict";

app.directive("retailAccountmanagerAccountmanagrassignmentGrid", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "Retail_BE_AccountManagerAssignmentAPIService", "Retail_BE_AccountManagerAssignmentService",
function (UtilsService, VRNotificationService, VRUIUtilsService, Retail_BE_AccountManagerAssignmentAPIService, Retail_BE_AccountManagerAssignmentService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountmanagrassignmentGrid = new AccountmanagrassignmentGrid($scope, ctrl, $attrs);
            accountmanagrassignmentGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Extensions/AccountManager/Templates/AccountManagerAccountManagerAssignmentGrid.html"
    };

    function AccountmanagrassignmentGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
        var gridAPI;
        var subViewEntity;


        function initializeController() {
            $scope.accountManagerAssignments = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveAPI());
                }
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (payload) {
                        subViewEntity = payload;
                        return Retail_BE_AccountManagerAssignmentAPIService.GetAccountManagerAssignments().then(function (response) {
                            $scope.accountManagerAssignments = response;
                        });

                    };
                    directiveAPI.onAccountManagerAssignmentAdded = function (accountManagerAssignment) {
                        gridAPI.itemAdded(accountManagerAssignment);
                    };
                    directiveAPI.getData = function () {
                        
                    };
                    return directiveAPI;
                };

            };
            defineMenuActions();
        }
        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editAccountManagerAssignment,
            }];
        }
        function editAccountManagerAssignment(accountManagerAssignment) {
            var onAccountManagerAssignmentUpdated = function (updatedItem) {
             
                gridAPI.itemUpdated(updatedItem);
            };
            var accountManagerAssignmentId = accountManagerAssignment.AccountManagerAssignementId;
            Retail_BE_AccountManagerAssignmentService.editAccountManagerAssignment(accountManagerAssignmentId, onAccountManagerAssignmentUpdated, subViewEntity);
        }
    

    }
    return directiveDefinitionObject;
}]);