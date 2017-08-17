"use strict";

app.directive("retailBeChargeableentitysettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var chargeableentitydefinitionsettings = new Chargeableentitydefinitionsettings($scope, ctrl);
            chargeableentitydefinitionsettings.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/ChargeableEntity/Templates/ChargeableEntitySettings.html"
    };

    function Chargeableentitydefinitionsettings($scope, ctrl) {

        this.initializeController = initializeController;

        var billingTransactionTypeSelectorAPI;
        var billingTransactionTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onBillingTransactionTypeSelectorReady = function (api) {
                billingTransactionTypeSelectorAPI = api;
                billingTransactionTypeSelectorReadyDeferred.resolve();
            };

            defineAPI();
        }
        function defineAPI() {
            var api = {};
            var selectedTransactionType;

            api.load = function (payload) {
                var promises = [];

                if (payload != undefined) {
                    selectedTransactionType = payload.TransactionTypeId;
                }

                var loadBillingTransactionTypeSelectorPromise = loadBillingTransactionTypeSelector();
                promises.push(loadBillingTransactionTypeSelectorPromise);
                return UtilsService.waitMultiplePromises(promises);
            };

            function loadBillingTransactionTypeSelector() {
                var billingTransactionTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                billingTransactionTypeSelectorReadyDeferred.promise.then(function () {
                    var billingTransactionTypeSelectorPayload;
                    if (selectedTransactionType != undefined) {
                        billingTransactionTypeSelectorPayload = {
                            selectedIds: selectedTransactionType
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(billingTransactionTypeSelectorAPI, billingTransactionTypeSelectorPayload, billingTransactionTypeSelectorLoadDeferred);
                });

                return billingTransactionTypeSelectorLoadDeferred.promise;
            }

            api.getData = function () {
                return {
                    $type: "Retail.BusinessEntity.Entities.ChargeableEntitySettings ,Retail.BusinessEntity.Entities",
                    TransactionTypeId: billingTransactionTypeSelectorAPI.getSelectedIds()
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
    return directiveDefinitionObject;

}]);