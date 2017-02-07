'use strict';
app.directive('vrInvoiceInvoicesettingRuntimeBillingperiodinvoicesettingpart', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '=',
                normalColNum: '@',
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new RowCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_Invoice/Directives/InvoiceSetting/MainExtensions/Templates/BillingPeriodInvoiceSettingPartTemplate.html';
            }

        };

        function RowCtor(ctrl, $scope) {
            var currentContext;
            var billingPeriodAPI;
            var billingPeriodReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onBillingPeriodReady = function (api) {
                    billingPeriodAPI = api;
                    billingPeriodReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.isBillingPeriodRequired = function () {
                    if (currentContext != undefined && currentContext.isBillingPeriodRequired != undefined)
                    {
                        return currentContext.isBillingPeriodRequired();
                    }
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        currentContext = payload.context;
                        if (payload.fieldValue != undefined) {
                            $scope.scopeModel.followBillingPeriod = payload.fieldValue.FollowBillingPeriod;

                        }
                    }

                    function loadBillingPeriod() {
                        var billingPeriodDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        billingPeriodReadyPromiseDeferred.promise.then(function () {
                            var billingPeriodDirectivePayload = {};
                            if (payload != undefined && payload.fieldValue != undefined)
                                billingPeriodDirectivePayload.billingPeriodEntity = payload.fieldValue.BillingPeriod;
                            VRUIUtilsService.callDirectiveLoad(billingPeriodAPI, billingPeriodDirectivePayload, billingPeriodDeferredLoadPromiseDeferred);
                        });
                        return billingPeriodDeferredLoadPromiseDeferred.promise;
                    }
                    return loadBillingPeriod();
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.Entities.BillingPeriodInvoiceSettingPart,Vanrise.Invoice.Entities",
                        FollowBillingPeriod: $scope.scopeModel.followBillingPeriod,
                        BillingPeriod: billingPeriodAPI.getData(),
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);