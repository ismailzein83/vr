'use strict';
app.directive('raDatatransformationSmstaxrulestep', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new SMSTaxRule(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Retail_RA/Directives/MappingSteps/SMSTaxRule/Templates/SMSTaxRuleStepTemplate.html';
            }

        };

        function SMSTaxRule(ctrl, $scope) {
            var ruleTypeName = "RA_SMSTaxRule";

            var ruleStepCommonDirectiveAPI;
            var ruleStepCommonDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var numberOfSMSsDirectiveAPI;
            var numberOfSMSsDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var totalAmountDirectiveAPI;
            var totalAmountDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var totalTaxValueDirectiveAPI;
            var totalTaxValueDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.onRuleStepCommonReady = function (api) {
                    ruleStepCommonDirectiveAPI = api;
                    ruleStepCommonDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onNumberOfSMSsDirectiveReady = function (api) {
                    numberOfSMSsDirectiveAPI = api;
                    numberOfSMSsDirectiveReadyPromiseDeferred.resolve();
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
                    var promises = [];

                    //loading RuleStepCommon directive
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
                    promises.push(loadRuleStepCommonDirectivePromiseDeferred.promise);
                    console.log(payload.stepDetails);
                    //loading NumberOfSMSs directive
                    var loadNumberOfSMSsDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    numberOfSMSsDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadNumberOfSMSs;
                        if (payload != undefined) {
                            payloadNumberOfSMSs = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadNumberOfSMSs.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadNumberOfSMSs.selectedRecords = payload.stepDetails.NumberOfSMSs;
                        }
                        VRUIUtilsService.callDirectiveLoad(numberOfSMSsDirectiveAPI, payloadNumberOfSMSs, loadNumberOfSMSsDirectivePromiseDeferred);
                    });
                    promises.push(loadNumberOfSMSsDirectivePromiseDeferred.promise);

                    //loading TotalAmount directive
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
                    promises.push(loadTotalAmountDirectivePromiseDeferred.promise);

                    //loading TotalTaxValue directive
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
                    promises.push(loadTotalTaxValueDirectivePromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "Retail.RA.Business.SMSTaxMappingStep, Retail.RA.Business",
                        NumberOfSMSs: numberOfSMSsDirectiveAPI != undefined ? numberOfSMSsDirectiveAPI.getData() : undefined,
                        TotalAmount: totalAmountDirectiveAPI != undefined ? totalAmountDirectiveAPI.getData() : undefined,
                        TotalTaxValue: totalTaxValueDirectiveAPI != undefined ? totalTaxValueDirectiveAPI.getData() : undefined
                    };
                    ruleStepCommonDirectiveAPI.setData(obj);
                    console.log(obj);
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