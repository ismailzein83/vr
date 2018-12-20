'use strict';
app.directive('raDatatransformationSmstaxrulestepPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new SMSTaxRuleStepCtor(ctrl, $scope);
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
                return '/Client/Modules/Retail_RA/Directives/MappingSteps/SMSTaxRule/Templates/SMSTaxRuleStepPreviewTemplate.html';
            }

        };

        function SMSTaxRuleStepCtor(ctrl, $scope) {
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
                            stepObj.numberOfSMSs = payload.stepDetails.NumberOfSMSs;
                            stepObj.totalAmount = payload.stepDetails.TotalAmount;
                            stepObj.totalTaxValue = payload.stepDetails.TotalTaxValue;
                            ctrl.numberOfSMSs = payload.stepDetails.NumberOfSMSs;
                            ctrl.totalAmount = payload.stepDetails.TotalAmount;
                            ctrl.totalTaxValue = payload.stepDetails.TotalTaxValue;
                        }

                    }
                    var promises = [];
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

                    promises.push(loadCommonDirectivePromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.applyChanges = function (changes) {
                    if (commonDirectiveAPI != undefined)
                        commonDirectiveAPI.applyChanges(changes);
                    stepObj.numberOfSMSs = changes.NumberOfSMSs;
                    stepObj.totalAmount = changes.TotalAmount;
                    stepObj.totalTaxValue = changes.TotalTaxValue;
                    ctrl.numberOfSMSs = changes.NumberOfSMSs;
                    ctrl.totalAmount = changes.TotalAmount;
                    ctrl.totalTaxValue = changes.TotalTaxValue;
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
                        stepDetails.NumberOfSMSs = stepObj.numberOfSMSs;
                        stepDetails.TotalAmount = stepObj.totalAmount;
                        stepDetails.TotalTaxValue = stepObj.totalTaxValue;
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