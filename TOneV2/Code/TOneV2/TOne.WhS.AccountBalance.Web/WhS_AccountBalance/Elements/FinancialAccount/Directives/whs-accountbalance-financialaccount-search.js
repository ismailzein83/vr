"use strict";

app.directive("whsAccountbalanceFinancialaccountSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'VR_AccountBalance_FinancialAccountService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, VR_AccountBalance_FinancialAccountService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var logSearch = new LogSearch($scope, ctrl, $attrs);
            logSearch.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_AccountBalance/Elements/FinancialAccount/Directives/Templates/FinancialAccountSearch.html"

    };

    function LogSearch($scope, ctrl, $attrs) {

        var gridAPI;
        var carrierAccountId;
        var carrierProfileId;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.addFinancialAccount = function () {
                var onFinancialAccountAdded = function (obj) {
                    gridAPI.onFinancialAccountAdded(obj);
                };
                VR_AccountBalance_FinancialAccountService.addFinancialAccount(carrierAccountId, carrierProfileId, onFinancialAccountAdded)
            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

          
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined)
                {
                    carrierAccountId = payload.carrierAccountId;
                    carrierProfileId = payload.carrierProfileId;
                }
                return gridAPI.loadGrid(payload);
            };
            api.onFnancialAccountAdded = function (financialAccount) {
                return gridAPI.itemAdded(financialAccount);
            };
            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }

        function getFilterObject() {
            var filter = {
                CarrierAccountId: carrierAccountId,
                CarrierProfileId: carrierProfileId
            };
            return filter;
        }
    }

    return directiveDefinitionObject;

}]);
