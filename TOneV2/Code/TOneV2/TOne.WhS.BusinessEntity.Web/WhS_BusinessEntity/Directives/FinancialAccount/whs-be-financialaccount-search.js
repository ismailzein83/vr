"use strict";

app.directive("whsBeFinancialaccountSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'WhS_BE_FinancialAccountService', 'WhS_BE_FinancialAccountAPIService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, WhS_BE_FinancialAccountService, WhS_BE_FinancialAccountAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new FinancialAccountSearch($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/FinancialAccount/Templates/FinancialAccountSearch.html"

    };

    function FinancialAccountSearch($scope, ctrl, $attrs) {

        var gridAPI;
        var carrierAccountId;
        var carrierProfileId;
        this.initializeController = initializeController;
        var context;
        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.hasAddFinancialAccountPermission = function () {
                return WhS_BE_FinancialAccountAPIService.HasAddFinancialAccountPermission();
            };

            $scope.scopeModel.addFinancialAccount = function () {
                var onFinancialAccountAdded = function (obj) {
                    if (context != undefined) {
                        context.canAddFinancialAccountToCarrier();
                    }
                    gridAPI.onFinancialAccountAdded(obj);
                };
                WhS_BE_FinancialAccountService.addFinancialAccount(carrierAccountId, carrierProfileId, onFinancialAccountAdded)
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };
            defineContext();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    carrierAccountId = payload.carrierAccountId;
                    carrierProfileId = payload.carrierProfileId;
                }
                var promises = [];
                promises.push(canAddFinancialAccountToCarrier());
                promises.push(gridAPI.loadGrid(getGridQuery()));

                return UtilsService.waitMultiplePromises(promises);

            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }
        function canAddFinancialAccountToCarrier() {
            return WhS_BE_FinancialAccountAPIService.CanAddFinancialAccountToCarrier(carrierAccountId ,carrierProfileId).then(function (response) {
                $scope.scopeModel.showAddButton = response;
            });
        }
        function getGridQuery() {

            var filter = {
                query: {
                    CarrierAccountId: carrierAccountId,
                    CarrierProfileId: carrierProfileId
                },
                context: context
            };
            return filter;
        }
        function defineContext() {
            context = {
                canAddFinancialAccountToCarrier: function () {
                    $scope.scopeModel.isLoading = true;
                    return canAddFinancialAccountToCarrier().finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                }
            };
        }
    }

    return directiveDefinitionObject;

}]);
