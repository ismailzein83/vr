"use strict";

app.directive("retailAccountmanagerAccountsubviewruntimeSearch", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "Retail_BE_AccountManagerAssignmentService",
function (UtilsService, VRNotificationService, VRUIUtilsService, Retail_BE_AccountManagerAssignmentService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var subViewsRuntimeSearch = new SubViewsRuntimeSearch($scope, ctrl, $attrs);
            subViewsRuntimeSearch.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Extensions/AccountManager/Templates/AccountSubViewRuntimeSearchTemplate.html"
    };

    function SubViewsRuntimeSearch($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
        var accountManagerAssignmentGridAPI;
        var subViewDefinitionEntity;
        
        function initializeController() {
           
            
            
            $scope.onAccountManagerAssignmentGridReady = function (api) {
                accountManagerAssignmentGridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveAPI());
                }
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.load = function (payload) {
                        subViewDefinitionEntity = payload
                        accountManagerAssignmentGridAPI.loadGrid(payload);
                    };
                    directiveAPI.getData = function () {
                       
                    };
                    return directiveAPI;
                };
                $scope.addAccountManagerAssignments = function () {
                   
                    var onAccountManagerAssignmentAdded = function (accountManagerAssignment) {
                        accountManagerAssignmentGridAPI.onAccountManagerAssignmentAdded(accountManagerAssignment);
                    }

                    Retail_BE_AccountManagerAssignmentService.addAccountManagerAssignments(subViewDefinitionEntity, onAccountManagerAssignmentAdded);

                };


            };
         
        }
    }
    return directiveDefinitionObject;
}]);