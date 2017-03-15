"use strict";

app.directive("whsAccountbalanceFinancialaccountSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'VR_AccountBalance_FinancialAccountService','WhS_AccountBalance_FinancialAccountAPIService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, VR_AccountBalance_FinancialAccountService, WhS_AccountBalance_FinancialAccountAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var logSearch = new FinancialAccountSearch($scope, ctrl, $attrs);
            logSearch.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_AccountBalance/Elements/FinancialAccount/Directives/Templates/FinancialAccountSearch.html"

    };

    function FinancialAccountSearch($scope, ctrl, $attrs) {

        var gridAPI;
        var carrierAccountId;
        var carrierProfileId;
        this.initializeController = initializeController;
        var context;
        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.addFinancialAccount = function () {
                var onFinancialAccountAdded = function (obj) {
                    if (context != undefined)
                    {
                        context.checkAllowAddFinancialAccount();
                    }
                    gridAPI.onFinancialAccountAdded(obj);
                };
                VR_AccountBalance_FinancialAccountService.addFinancialAccount(carrierAccountId, carrierProfileId, onFinancialAccountAdded)
            }

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };
            defineContext();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined)
                {
                    carrierAccountId = payload.carrierAccountId;
                    carrierProfileId = payload.carrierProfileId;
                }
                var promises = [];
                promises.push(checkAllowAddFinancialAccount());
                promises.push(gridAPI.loadGrid(getGridQuery()));

                return UtilsService.waitMultiplePromises(promises);
               
            };
            
            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }
        function checkAllowAddFinancialAccount() {
            return WhS_AccountBalance_FinancialAccountAPIService.CheckCarrierAllowAddFinancialAccounts(carrierProfileId, carrierAccountId).then(function (response) {
                $scope.scopeModel.showAddButton = response;
            });
        }
        function getGridQuery() {

            var filter = {
                query:{
                    CarrierAccountId: carrierAccountId,
                    CarrierProfileId: carrierProfileId
                },
                context: context
            };
            return filter;
        }
        function defineContext()
        {
            context = {
                checkAllowAddFinancialAccount: function () {
                    $scope.scopeModel.isLoading = true;
                    return checkAllowAddFinancialAccount().finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                }
            };
        }
    }

    return directiveDefinitionObject;

}]);
