"use strict";

app.directive("retailBeAccountmanagerAccountmanagrassignmentGrid", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "Retail_BE_AccountManagerAssignmentAPIService", "Retail_BE_AccountManagerAssignmentService",
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
        var searchPayload;
        var accountManagerAssignementDefinitionId;
        var accountId;
        var accountBeDefinitionId;
        var accountManagerDefinitionId;


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
                        searchPayload = payload;
                        if (searchPayload != undefined) {
                            accountId= searchPayload.accountId;
                            accountBeDefinitionId = searchPayload.accountBEDefinitionId;
                            accountManagerDefinitionId = searchPayload.accountManagerDefinitionId;
                            

                            if (searchPayload.accountManagerSubViewDefinition != undefined && searchPayload.accountManagerSubViewDefinition.Settings != undefined)
                                accountManagerAssignementDefinitionId = searchPayload.accountManagerSubViewDefinition.Settings.AccountManagerAssignementDefinitionId;
                            if (searchPayload.accountManagerAssignementDefinitionId != undefined)
                                accountManagerAssignementDefinitionId = searchPayload.accountManagerAssignementDefinitionId;
                        }
                        if (accountBeDefinitionId != undefined) 
                            $scope.showAccountManagerName = true;
                        else 
                            $scope.showAccountName = true;
                        return gridAPI.retrieveData(getGridQuery());
                    };
                    directiveAPI.onAccountManagerAssignmentAdded = function (accountManagerAssignment) {
                        gridAPI.itemAdded(accountManagerAssignment);
                    };
                    directiveAPI.getData = function () {
                        
                    };
                    return directiveAPI;
                }

            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_BE_AccountManagerAssignmentAPIService.GetAccountManagerAssignments(dataRetrievalInput)
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
                clicked: editAccountManagerAssignment,
            }];
        }
        function editAccountManagerAssignment(accountManagerAssignment) {
            var onAccountManagerAssignmentUpdated = function (updatedItem) {
             
                gridAPI.itemUpdated(updatedItem);
            };
            var accountManagerAssignmentId = accountManagerAssignment.AccountManagerAssignementId;
            Retail_BE_AccountManagerAssignmentService.editAccountManagerAssignment(accountManagerAssignmentId, onAccountManagerAssignmentUpdated, accountManagerDefinitionId, accountManagerAssignementDefinitionId,accountId);
        }
        function getGridQuery() {
            var Query = {
                AccountManagerId: searchPayload.accountManagerId,
                AccountManagerAssignementDefinitionId: accountManagerAssignementDefinitionId,
                AccountId: accountId,
                AccountBEDefinitionId: accountBeDefinitionId
            };
            return Query;
        }
    

    }
    return directiveDefinitionObject;
}]);