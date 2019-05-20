'use strict';
app.directive('raDatatransformationPostpaidtaxrulestepPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new TaxRuleStepCtor(ctrl, $scope);
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
                return '/Client/Modules/Retail_RA/Directives/MappingSteps/PostpaidTaxRule/Templates/PostpaidTaxRuleStepPreviewTemplate.html';
            }

        };

        function TaxRuleStepCtor(ctrl, $scope) {
            var stepObj = {};
            var commonDirectiveAPI;
            var commonDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                ctrl.onCommonDirectiveReady = function (api) {
                    commonDirectiveAPI = api;
                    commonDirectiveReadyPromiseDeferred.resolve();
                };
                ctrl.ruleFieldsMappings = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.ruleFieldsMappings.length = 0;
                    if (payload != undefined) {
                        if (payload.stepDetails != undefined) {
                            stepObj.durationInSeconds = payload.stepDetails.DurationInSeconds;
                            stepObj.totalVoiceAmount = payload.stepDetails.TotalVoiceAmount;
                            stepObj.totalVoiceTaxValue = payload.stepDetails.TotalVoiceTaxValue;
                            ctrl.durationInSeconds = payload.stepDetails.DurationInSeconds;
                            ctrl.totalVoiceAmount = payload.stepDetails.TotalVoiceAmount;
                            ctrl.totalVoiceTaxValue = payload.stepDetails.TotalVoiceTaxValue;

                            stepObj.numberOfSMS = payload.stepDetails.NumberOfSMS;
                            stepObj.totalSMSAmount = payload.stepDetails.TotalSMSAmount;
                            stepObj.totalSMSTaxValue = payload.stepDetails.TotalSMSTaxValue;
                            ctrl.numberOfSMS = payload.stepDetails.NumberOfSMS;
                            ctrl.totalSMSAmount = payload.stepDetails.TotalSMSAmount;
                            ctrl.totalSMSTaxValue = payload.stepDetails.TotalSMSTaxValue;


                            stepObj.totalTransactionAmount = payload.stepDetails.TotalTransactionAmount;
                            stepObj.totalTransactionTaxValue = payload.stepDetails.TotalTransactionTaxValue;
                            ctrl.totalTransactionAmount = payload.stepDetails.TotalTransactionAmount;
                            ctrl.totalTransactionTaxValue = payload.stepDetails.TotalTransactionTaxValue;
                        }

                    }
                    function loadCommonDirective() {
                        var loadCommonDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        commonDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payloadCommon;
                            if (payload != undefined) {
                                payloadCommon = {};
                                if (payload != undefined)
                                    payloadCommon.stepDetails = payload.stepDetails;
                            }
                            VRUIUtilsService.callDirectiveLoad(commonDirectiveAPI, payloadCommon, loadCommonDirectivePromiseDeferred);
                        });
                        return loadCommonDirectivePromiseDeferred.promise;
                    }


                    var rootPromiseNode = {
                        promises: [loadCommonDirective()]
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.applyChanges = function (changes) {
                    if (commonDirectiveAPI != undefined)
                        commonDirectiveAPI.applyChanges(changes);

                    stepObj.durationInSeconds = changes.DurationInSeconds;
                    stepObj.totalVoiceAmount = changes.TotalVoiceAmount;
                    stepObj.totalVoiceTaxValue = changes.TotalVoiceTaxValue;
                    ctrl.durationInSeconds = changes.DurationInSeconds;
                    ctrl.totalVoiceAmount = changes.TotalVoiceAmount;
                    ctrl.totalVoiceTaxValue = changes.TotalVoiceTaxValue;

                    stepObj.numberOfSMS = changes.NumberOfSMS;
                    stepObj.totalSMSAmount = changes.TotalSMSAmount;
                    stepObj.totalSMSTaxValue = changes.TotalSMSTaxValue;
                    ctrl.numberOfSMS = changes.NumberOfSMS;
                    ctrl.totalSMSAmount = changes.TotalSMSAmount;
                    ctrl.totalSMSTaxValue = changes.TotalSMSTaxValue;


                    stepObj.totalTransactionAmount = changes.TotalTransactionAmount;
                    stepObj.totalTransactionTaxValue = changes.TotalTransactionTaxValue;
                    ctrl.totalTransactionAmount = changes.TotalTransactionAmount;
                    ctrl.totalTransactionTaxValue = changes.TotalTransactionTaxValue;
                 
                };

                api.checkValidation = function () {
                    var validate;
                    if (commonDirectiveAPI != undefined)
                        validate = commonDirectiveAPI.checkValidation();
                    if (validate == undefined)
                        validate = checkValidation();
                    return validate;
                };

                api.getData = function () {
                    var stepDetails = commonDirectiveAPI != undefined ? commonDirectiveAPI.getData() : undefined;
                    if (stepDetails != undefined) {
                        stepDetails.DurationInSeconds = stepObj.durationInSeconds;
                        stepDetails.TotalVoiceAmount = stepObj.totalVoiceAmount;
                        stepDetails.TotalVoiceTaxValue = stepObj.totalVoiceTaxValue;

                        stepDetails.NumberOfSMS = stepObj.numberOfSMS;
                        stepDetails.TotalSMSAmount = stepObj.totalSMSAmount;
                        stepDetails.TotalSMSTaxValue = stepObj.totalSMSTaxValue;

                        stepDetails.TotalTransactionAmount = stepObj.totalTransactionAmount;
                        stepDetails.TotalTransactionTaxValue = stepObj.totalTransactionTaxValue;
                    }
                    return stepDetails;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function checkValidation() {
                return null;
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);