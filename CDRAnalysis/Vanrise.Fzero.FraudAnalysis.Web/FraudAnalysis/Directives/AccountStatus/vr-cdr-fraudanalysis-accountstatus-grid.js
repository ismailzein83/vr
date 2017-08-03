"use strict";

app.directive("vrCdrFraudanalysisAccountstatusGrid", ["UtilsService", "VRNotificationService", "Fzero_FraudAnalysis_AccountStatusAPIService", "Fzero_FraudAnalysis_AccountStatusService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, Fzero_FraudAnalysis_AccountStatusAPIService, Fzero_FraudAnalysis_AccountStatusService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountStatusGrid = new AccountStatusGrid($scope, ctrl, $attrs);
            accountStatusGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/FraudAnalysis/Directives/AccountStatus/Templates/AccountStatusGridTemplate.html"

    };

    function AccountStatusGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.accountStatuses = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {

                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onAccountStatusAdded = function (accountStatusObject) {
                        gridAPI.itemAdded(accountStatusObject);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Fzero_FraudAnalysis_AccountStatusAPIService.GetAccountStatusesData(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editAccountStatus,
                haspermission: hasEditAccountStatusPermission
            }
            ,

            {
                name: "Remove",
                clicked: deleteAccountStatus,
                haspermission: hasDeleteAccountStatusPermission
            }


            ];
        }


        function hasDeleteAccountStatusPermission() {
            return Fzero_FraudAnalysis_AccountStatusAPIService.HasDeleteAccountStatusPermission();
        }

        function deleteAccountStatus(accountStatusObj) {
            var onAccountStatusDeleted = function (accountStatusObj) {
                gridAPI.itemDeleted(accountStatusObj);
            };

            Fzero_FraudAnalysis_AccountStatusService.deleteAccountStatus(accountStatusObj, onAccountStatusDeleted);
        }




        function hasEditAccountStatusPermission() {
            return Fzero_FraudAnalysis_AccountStatusAPIService.HasEditAccountStatusPermission();
        }

        function editAccountStatus(accountStatusObj) {
            var onAccountStatusUpdated = function (accountStatusObj) {
                gridAPI.itemUpdated(accountStatusObj);
            };

            Fzero_FraudAnalysis_AccountStatusService.editAccountStatus(accountStatusObj.Entity.AccountNumber, onAccountStatusUpdated);
        }

    }

    return directiveDefinitionObject;

}]);
