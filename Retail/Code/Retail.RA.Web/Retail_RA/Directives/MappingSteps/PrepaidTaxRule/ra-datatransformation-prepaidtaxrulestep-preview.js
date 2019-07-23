'use strict';
app.directive('raDatatransformationPrepaidtaxrulestepPreview', ['UtilsService', 'VRUIUtilsService',
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
                return '/Client/Modules/Retail_RA/Directives/MappingSteps/PrepaidTaxRule/Templates/PrepaidTaxRuleStepPreviewTemplate.html';
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
                            stepObj.totalTopUpAmount = payload.stepDetails.TotalTopUpAmount;
                            stepObj.totalTopUpTaxValue = payload.stepDetails.TotalTopUpTaxValue;
                            ctrl.totalTopUpAmount = payload.stepDetails.TotalTopUpAmount;
                            ctrl.totalTopUpTaxValue = payload.stepDetails.TotalTopUpTaxValue;
                        }
                    }

                    function loadCommonDirective() {
                        var loadCommonDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        commonDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payloadCommon;
                            if (payload != undefined) {
                                payloadCommon = {
                                    stepDetails: payload.stepDetails
                                };
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

                    stepObj.totalTopUpAmount = changes.TotalTopUpAmount;
                    stepObj.totalTopUpTaxValue = changes.TotalTopUpTaxValue;
                    ctrl.totalTopUpAmount = changes.TotalTopUpAmount;
                    ctrl.totalTopUpTaxValue = changes.TotalTopUpTaxValue;
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
                        stepDetails.TotalTopUpAmount = stepObj.totalTopUpAmount;
                        stepDetails.TotalTopUpTaxValue = stepObj.totalTopUpTaxValue;
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