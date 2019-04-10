'use strict';
app.directive('raDatatransformationTransactiontaxrulestep', ['UtilsService', 'VRUIUtilsService',
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
            templateUrl:'/Client/Modules/Retail_RA/Directives/MappingSteps/TransactionTaxRule/Templates/TransactionTaxRuleStepTemplate.html'
        };

        function taxRule(ctrl, $scope) {
            var ruleTypeName = "RA_TransactionTaxRule";

            var ruleStepCommonDirectiveAPI;
            var ruleStepCommonDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var totalAmountDirectiveAPI;
            var totalAmountDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var totalTaxValueDirectiveAPI;
            var totalTaxValueDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.onRuleStepCommonReady = function (api) {
                    ruleStepCommonDirectiveAPI = api;
                    ruleStepCommonDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onTotalAmountDirectiveReady = function (api) {
                    totalAmountDirectiveAPI = api;
                    totalAmountDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onTotalTaxValueDirectiveReady = function (api) {
                    totalTaxValueDirectiveAPI = api;
                    totalTaxValueDirectiveReadyPromiseDeferred.resolve();
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
                            if (payload != undefined && payload.context != undefined)
                                payloadRuleStep.context = payload.context;
                            if (payload != undefined && payload.stepDetails) {
                                payloadRuleStep.ruleFieldsMappings = payload.stepDetails.RuleFieldsMappings;
                                payloadRuleStep.ruleObjectsMappings = payload.stepDetails.RuleObjectsMappings;
                                payloadRuleStep.effectiveTime = payload.stepDetails.EffectiveTime;
                                payloadRuleStep.isEffectiveInFuture = payload.stepDetails.IsEffectiveInFuture;
                                payloadRuleStep.ruleDefinitionId = payload.stepDetails.RuleDefinitionId;
                                payloadRuleStep.ruleId = payload.stepDetails.RuleId;
                            }
                            VRUIUtilsService.callDirectiveLoad(ruleStepCommonDirectiveAPI, payloadRuleStep, loadRuleStepCommonDirectivePromiseDeferred);
                        });
                        return loadRuleStepCommonDirectivePromiseDeferred.promise;
                    }

                    function loadTotalAmountDirective() {
                        var loadTotalAmountDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        totalAmountDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payloadTotalAmount;
                            if (payload != undefined) {
                                payloadTotalAmount = {};
                                if (payload != undefined && payload.context != undefined)
                                    payloadTotalAmount.context = payload.context;
                                if (payload != undefined && payload.stepDetails != undefined)
                                    payloadTotalAmount.selectedRecords = payload.stepDetails.TotalAmount;
                            }
                            VRUIUtilsService.callDirectiveLoad(totalAmountDirectiveAPI, payloadTotalAmount, loadTotalAmountDirectivePromiseDeferred);
                        });
                        return loadTotalAmountDirectivePromiseDeferred.promise;
                    }

                    function loadTotalTaxValueDirective() {
                        var loadTotalTaxValueDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        totalTaxValueDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payloadTotalTaxValue;
                            if (payload != undefined) {
                                payloadTotalTaxValue = {};
                                payloadTotalTaxValue.context = payload.context;
                                if (payload.stepDetails != undefined) {
                                    payloadTotalTaxValue.selectedRecords = payload.stepDetails.TotalTaxValue;
                                }
                            }
                            VRUIUtilsService.callDirectiveLoad(totalTaxValueDirectiveAPI, payloadTotalTaxValue, loadTotalTaxValueDirectivePromiseDeferred);
                        });
                        return loadTotalTaxValueDirectivePromiseDeferred.promise;
                    }
                 

                    var rootPromise = {
                        promises: [loadCommonDirective(), loadTotalAmountDirective(), loadTotalTaxValueDirective()]
                    };
                    return UtilsService.waitPromiseNode(rootPromise);
                };

                api.getData = function () {
                    var obj = {
                        $type: "Retail.RA.Business.TransactionTaxMappingStep, Retail.RA.Business",
                        TotalAmount: totalAmountDirectiveAPI != undefined ? totalAmountDirectiveAPI.getData() : undefined,
                        TotalTaxValue: totalTaxValueDirectiveAPI != undefined ? totalTaxValueDirectiveAPI.getData() : undefined
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