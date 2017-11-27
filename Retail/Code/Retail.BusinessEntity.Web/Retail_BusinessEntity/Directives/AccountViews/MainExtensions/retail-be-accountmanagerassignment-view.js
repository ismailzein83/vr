"use strict";

app.directive("retailBeAccountmanagerassignmentView", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "Retail_BE_AccountManagerAssignmentService","Retail_BE_AccountManagerAssignmentAPIService",
function (UtilsService, VRNotificationService, VRUIUtilsService, Retail_BE_AccountManagerAssignmentService,Retail_BE_AccountManagerAssignmentAPIService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountManagerAssignmnetView = new AccountManagerAssignmnetView($scope, ctrl, $attrs);
            accountManagerAssignmnetView.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountViews/MainExtensions/Templates/AccountManagerAssignmentViewTemplate.html'
    };

    function AccountManagerAssignmnetView($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
        var accountManagerAssignmentGridAPI;
        var gridPayload;
        var accountManagerDefinitionId;
        var accountManagerAssignementDefinitionId;
        var accountId;
        var accountBEDefinitionId;

        function initializeController() {
            $scope.onAccountManagerAssignmentGridReady = function (api) {
                accountManagerAssignmentGridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveAPI());
                }
            };
            $scope.addAccountManagerAssignments = function () {
                var onAccountManagerAssignmentAdded = function (accountManagerAssignment) {
                    accountManagerAssignmentGridAPI.onAccountManagerAssignmentAdded(accountManagerAssignment);
                };
                Retail_BE_AccountManagerAssignmentService.addAccountManagerAssignments(accountManagerDefinitionId,undefined, accountManagerAssignementDefinitionId, onAccountManagerAssignmentAdded,accountId);
            };
            function getDirectiveAPI() {
                var directiveAPI = {};
                directiveAPI.load = function (payload) {
                    gridPayload = payload;
                    if (gridPayload != undefined) {
                        accountId = gridPayload.parentAccountId;
                        accountBEDefinitionId = gridPayload.accountBEDefinitionId;
                    }
                    if (payload != undefined) { 
                        Retail_BE_AccountManagerAssignmentAPIService.GetAccountManagerDefInfo(payload.accountBEDefinitionId).then(function (response) {
                            if (response != undefined) {
                                accountManagerDefinitionId = response.AccountManagerDefinitionId;
                                if (response.AccountManagerAssignmentDefinition != undefined)
                                    accountManagerAssignementDefinitionId = response.AccountManagerAssignmentDefinition.AccountManagerAssignementDefinitionId;

                            }
                            accountManagerAssignmentGridAPI.loadGrid(getGridPayload());
                    }); 
                }
                
                   
                };
                directiveAPI.getData = function () {

                };
                return directiveAPI;
            }
            function getGridPayload() {
                var payload = {
                    accountBEDefinitionId: accountBEDefinitionId,
                    accountId: gridPayload.parentAccountId,
                    accountManagerDefinitionId: accountManagerDefinitionId,
                    accountManagerAssignementDefinitionId: accountManagerAssignementDefinitionId

                };
                return payload;
            }
        }
    }
    return directiveDefinitionObject;
}]);