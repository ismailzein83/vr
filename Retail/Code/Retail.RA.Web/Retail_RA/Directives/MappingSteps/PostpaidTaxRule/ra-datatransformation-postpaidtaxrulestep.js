    'use strict';
app.directive('raDatatransformationPostpaidtaxrulestep', ['UtilsService', 'VRUIUtilsService',
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
            templateUrl: '/Client/Modules/Retail_RA/Directives/MappingSteps/PostpaidTaxRule/Templates/PostpaidTaxRuleStepTemplate.html'
        };

        function taxRule(ctrl, $scope) {
            var ruleTypeName = "RA_PostpaidTaxRule";

            var ruleStepCommonDirectiveAPI;
            var ruleStepCommonDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var durationInSecondsDirectiveAPI;
            var durationInSecondsDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var totalVoiceAmountDirectiveAPI;
            var totalVoiceAmountDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var totalVoiceTaxValueDirectiveAPI;
            var totalVoiceTaxValueDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var numberOfSMSDirectiveAPI;
            var numberOfSMSDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var totalSMSAmountDirectiveAPI;
            var totalSMSAmountDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var totalSMSTaxValueDirectiveAPI;
            var totalSMSTaxValueDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var totalTransactionAmountDirectiveAPI;
            var totalTransactionAmountDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var totalTransactionTaxValueDirectiveAPI;
            var totalTransactionTaxValueDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.onRuleStepCommonReady = function (api) {
                    ruleStepCommonDirectiveAPI = api;
                    ruleStepCommonDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onDurationInSecondsDirectiveReady = function (api) {
                    durationInSecondsDirectiveAPI = api;
                    durationInSecondsDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onTotalVoiceAmountDirectiveReady = function (api) {
                    totalVoiceAmountDirectiveAPI = api;
                    totalVoiceAmountDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onTotalVoiceTaxValueDirectiveReady = function (api) {
                    totalVoiceTaxValueDirectiveAPI = api;
                    totalVoiceTaxValueDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onNumberOfSMSDirectiveReady = function (api) {
                    numberOfSMSDirectiveAPI = api;
                    numberOfSMSDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onTotalSMSAmountDirectiveReady = function (api) {
                    totalSMSAmountDirectiveAPI = api;
                    totalSMSAmountDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onTotalSMSTaxValueDirectiveReady = function (api) {
                    totalSMSTaxValueDirectiveAPI = api;
                    totalSMSTaxValueDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onTotalTransactionAmountDirectiveReady = function (api) {
                    totalTransactionAmountDirectiveAPI = api;
                    totalTransactionAmountDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onTotalTransactionTaxValueDirectiveReady = function (api) {
                    totalTransactionTaxValueDirectiveAPI = api;
                    totalTransactionTaxValueDirectiveReadyPromiseDeferred.resolve();
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

                    function loadTotalVoiceAmountDirective() {
                        var loadTotalAmountDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        totalVoiceAmountDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payloadTotalAmount;
                            if (payload != undefined) {
                                payloadTotalAmount = {};
                                if (payload != undefined && payload.context != undefined)
                                    payloadTotalAmount.context = payload.context;
                                if (payload != undefined && payload.stepDetails != undefined)
                                    payloadTotalAmount.selectedRecords = payload.stepDetails.TotalVoiceAmount;
                            }
                            VRUIUtilsService.callDirectiveLoad(totalVoiceAmountDirectiveAPI, payloadTotalAmount, loadTotalAmountDirectivePromiseDeferred);
                        });
                        return loadTotalAmountDirectivePromiseDeferred.promise;
                    }

                    function loadTotalDurationDirective() {
                        var loadTotalDurationDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        durationInSecondsDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payloadTotalAmount;
                            if (payload != undefined) {
                                payloadTotalAmount = {};
                                if (payload != undefined && payload.context != undefined)
                                    payloadTotalAmount.context = payload.context;
                                if (payload != undefined && payload.stepDetails != undefined)
                                    payloadTotalAmount.selectedRecords = payload.stepDetails.DurationInSeconds;
                            }
                            VRUIUtilsService.callDirectiveLoad(durationInSecondsDirectiveAPI, payloadTotalAmount, loadTotalDurationDirectivePromiseDeferred);
                        });
                        return loadTotalDurationDirectivePromiseDeferred.promise;
                    }

                    function loadTotalVoiceTaxValueDirective() {
                        var loadTotalTaxValueDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        totalVoiceTaxValueDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payloadTotalTaxValue;
                            if (payload != undefined) {
                                payloadTotalTaxValue = {};
                                payloadTotalTaxValue.context = payload.context;
                                if (payload.stepDetails != undefined) {
                                    payloadTotalTaxValue.selectedRecords = payload.stepDetails.TotalVoiceTaxValue;
                                }
                            }
                            VRUIUtilsService.callDirectiveLoad(totalVoiceTaxValueDirectiveAPI, payloadTotalTaxValue, loadTotalTaxValueDirectivePromiseDeferred);
                        });
                        return loadTotalTaxValueDirectivePromiseDeferred.promise;
                    }

                    function loadTotalSMSAmountDirective() {
                        var loadTotalAmountDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        totalSMSAmountDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payloadTotalAmount;
                            if (payload != undefined) {
                                payloadTotalAmount = {};
                                if (payload != undefined && payload.context != undefined)
                                    payloadTotalAmount.context = payload.context;
                                if (payload != undefined && payload.stepDetails != undefined)
                                    payloadTotalAmount.selectedRecords = payload.stepDetails.TotalSMSAmount;
                            }
                            VRUIUtilsService.callDirectiveLoad(totalSMSAmountDirectiveAPI, payloadTotalAmount, loadTotalAmountDirectivePromiseDeferred);
                        });
                        return loadTotalAmountDirectivePromiseDeferred.promise;
                    }

                    function loadNumberOfSMSDirective() {
                        var loadNumberOfSMSDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        numberOfSMSDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payloadTotalAmount;
                            if (payload != undefined) {
                                payloadTotalAmount = {};
                                if (payload != undefined && payload.context != undefined)
                                    payloadTotalAmount.context = payload.context;
                                if (payload != undefined && payload.stepDetails != undefined)
                                    payloadTotalAmount.selectedRecords = payload.stepDetails.NumberOfSMS;
                            }
                            VRUIUtilsService.callDirectiveLoad(numberOfSMSDirectiveAPI, payloadTotalAmount, loadNumberOfSMSDirectivePromiseDeferred);
                        });
                        return loadNumberOfSMSDirectivePromiseDeferred.promise;
                    }

                    function loadTotalSMSTaxValueDirective() {
                        var loadTotalTaxValueDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        totalSMSTaxValueDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payloadTotalTaxValue;
                            if (payload != undefined) {
                                payloadTotalTaxValue = {};
                                payloadTotalTaxValue.context = payload.context;
                                if (payload.stepDetails != undefined) {
                                    payloadTotalTaxValue.selectedRecords = payload.stepDetails.TotalSMSTaxValue;
                                }
                            }
                            VRUIUtilsService.callDirectiveLoad(totalSMSTaxValueDirectiveAPI, payloadTotalTaxValue, loadTotalTaxValueDirectivePromiseDeferred);
                        });
                        return loadTotalTaxValueDirectivePromiseDeferred.promise;
                    }

                    function loadTotalTransactionAmountDirective() {
                        var loadTotalAmountDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        totalTransactionAmountDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payloadTotalAmount;
                            if (payload != undefined) {
                                payloadTotalAmount = {};
                                if (payload != undefined && payload.context != undefined)
                                    payloadTotalAmount.context = payload.context;
                                if (payload != undefined && payload.stepDetails != undefined)
                                    payloadTotalAmount.selectedRecords = payload.stepDetails.TotalTransactionAmount;
                            }
                            VRUIUtilsService.callDirectiveLoad(totalTransactionAmountDirectiveAPI, payloadTotalAmount, loadTotalAmountDirectivePromiseDeferred);
                        });
                        return loadTotalAmountDirectivePromiseDeferred.promise;
                    }

                    function loadTotalTransactionTaxValueDirective() {
                        var loadTotalTaxValueDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        totalTransactionTaxValueDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payloadTotalTaxValue;
                            if (payload != undefined) {
                                payloadTotalTaxValue = {};
                                payloadTotalTaxValue.context = payload.context;
                                if (payload.stepDetails != undefined) {
                                    payloadTotalTaxValue.selectedRecords = payload.stepDetails.TotalTransactionTaxValue;
                                }
                            }
                            VRUIUtilsService.callDirectiveLoad(totalTransactionTaxValueDirectiveAPI, payloadTotalTaxValue, loadTotalTaxValueDirectivePromiseDeferred);
                        });
                        return loadTotalTaxValueDirectivePromiseDeferred.promise;
                    }



                    var rootPromise = {
                        promises: [loadCommonDirective(), loadTotalVoiceAmountDirective(), loadTotalDurationDirective(), loadTotalVoiceTaxValueDirective(),
                        loadTotalSMSAmountDirective(), loadNumberOfSMSDirective(), loadTotalSMSTaxValueDirective(),
                        loadTotalTransactionAmountDirective(), loadTotalTransactionTaxValueDirective()
                        ]
                    };
                    return UtilsService.waitPromiseNode(rootPromise);
                };

                api.getData = function () {
                    var obj = {
                        $type: "Retail.RA.Business.PostpaidTaxMappingStep, Retail.RA.Business",
                        DurationInSeconds: durationInSecondsDirectiveAPI != undefined ? durationInSecondsDirectiveAPI.getData() : undefined,
                        TotalVoiceAmount: totalVoiceAmountDirectiveAPI != undefined ? totalVoiceAmountDirectiveAPI.getData() : undefined,
                        TotalVoiceTaxValue: totalVoiceTaxValueDirectiveAPI != undefined ? totalVoiceTaxValueDirectiveAPI.getData() : undefined,
                        NumberOfSMS: numberOfSMSDirectiveAPI != undefined ? numberOfSMSDirectiveAPI.getData() : undefined,
                        TotalSMSAmount: totalSMSAmountDirectiveAPI != undefined ? totalSMSAmountDirectiveAPI.getData() : undefined,
                        TotalSMSTaxValue: totalSMSTaxValueDirectiveAPI != undefined ? totalSMSTaxValueDirectiveAPI.getData() : undefined,
                        TotalTransactionAmount: totalTransactionAmountDirectiveAPI != undefined ? totalTransactionAmountDirectiveAPI.getData() : undefined,
                        TotalTransactionTaxValue: totalTransactionTaxValueDirectiveAPI != undefined ? totalTransactionTaxValueDirectiveAPI.getData() : undefined
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