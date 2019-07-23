'use strict';
app.directive('raDatatransformationPrepaidtaxrulestep', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new taxRule(ctrl, $scope);
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
            templateUrl: '/Client/Modules/Retail_RA/Directives/MappingSteps/PrepaidTaxRule/Templates/PrepaidTaxRuleStepTemplate.html'
        };

        function taxRule(ctrl, $scope) {
            var ruleTypeName = "RA_Retail_PrepaidTaxRule";

            var ruleStepCommonDirectiveAPI;
            var ruleStepCommonDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var totalTopUpAmountDirectiveAPI;
            var totalTopUpAmountDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var totalTopUpTaxValueDirectiveAPI;
            var totalTopUpTaxValueDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.onRuleStepCommonReady = function (api) {
                    ruleStepCommonDirectiveAPI = api;
                    ruleStepCommonDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onTotalTopUpAmountDirectiveReady = function (api) {
                    totalTopUpAmountDirectiveAPI = api;
                    totalTopUpAmountDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onTotalTopUpTaxValueDirectiveReady = function (api) {
                    totalTopUpTaxValueDirectiveAPI = api;
                    totalTopUpTaxValueDirectiveReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    function loadCommonDirective() {
                        var loadRuleStepCommonDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        ruleStepCommonDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payloadRuleStep = { ruleTypeName: ruleTypeName };
                            if (payload != undefined) {
                                payloadRuleStep.context = payload.context;
                                if (payload.stepDetails != undefined) {
                                    payloadRuleStep.ruleFieldsMappings = payload.stepDetails.RuleFieldsMappings;
                                    payloadRuleStep.ruleObjectsMappings = payload.stepDetails.RuleObjectsMappings;
                                    payloadRuleStep.effectiveTime = payload.stepDetails.EffectiveTime;
                                    payloadRuleStep.isEffectiveInFuture = payload.stepDetails.IsEffectiveInFuture;
                                    payloadRuleStep.ruleDefinitionId = payload.stepDetails.RuleDefinitionId;
                                    payloadRuleStep.ruleId = payload.stepDetails.RuleId;
                                }
                            }

                            VRUIUtilsService.callDirectiveLoad(ruleStepCommonDirectiveAPI, payloadRuleStep, loadRuleStepCommonDirectivePromiseDeferred);
                        });
                        return loadRuleStepCommonDirectivePromiseDeferred.promise;
                    }

                    function loadTotalTopUpAmountDirective() {
                        var loadTotalTopUpAmountDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        totalTopUpAmountDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payloadTotalAmount;
                            if (payload != undefined) {
                                payloadTotalAmount = {};
                                payloadTotalAmount.context = payload.context;
                                payloadTotalAmount.selectedRecords = payload.stepDetails != undefined ? payload.stepDetails.TotalTransactionAmount : undefined;
                            }
                            VRUIUtilsService.callDirectiveLoad(totalTopUpAmountDirectiveAPI, payloadTotalAmount, loadTotalTopUpAmountDirectivePromiseDeferred);
                        });
                        return loadTotalTopUpAmountDirectivePromiseDeferred.promise;
                    }

                    function loadTotalTopUpTaxValueDirective() {
                        var loadTotalTaxValueDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        totalTopUpTaxValueDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payloadTotalTaxValue;
                            if (payload != undefined) {
                                payloadTotalTaxValue = {};
                                payloadTotalTaxValue.context = payload.context;
                                payloadTotalTaxValue.selectedRecords = payload.stepDetails != undefined ? payload.stepDetails.TotalTransactionTaxValue : undefined;
                            }
                            VRUIUtilsService.callDirectiveLoad(totalTopUpTaxValueDirectiveAPI, payloadTotalTaxValue, loadTotalTaxValueDirectivePromiseDeferred);
                        });
                        return loadTotalTaxValueDirectivePromiseDeferred.promise;
                    }

                    var rootPromiseNode = {
                        promises: [loadCommonDirective(), loadTotalTopUpAmountDirective(), loadTotalTopUpTaxValueDirective()]
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    var obj = {
                        $type: "Retail.RA.Business.PrepaidTaxMappingStep, Retail.RA.Business",
                        TotalTopUpAmount: totalTopUpAmountDirectiveAPI != undefined ? totalTopUpAmountDirectiveAPI.getData() : undefined,
                        TotalTopUpTaxValue: totalTopUpTaxValueDirectiveAPI != undefined ? totalTopUpTaxValueDirectiveAPI.getData() : undefined
                    };
                    ruleStepCommonDirectiveAPI.setData(obj);
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);