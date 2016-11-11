'use strict';
app.directive('vrWhsBeDatatransformationCustomerzoneratePreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new CustomerZoneRatePreviewStepCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/TransformationSteps/CustomerZoneRateStep/Templates/CustomerZoneRateStepPreviewTemplate.html';
            }

        };

        function CustomerZoneRatePreviewStepCtor(ctrl, $scope) {
            var stepObj = {};

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        if (payload.stepDetails != undefined) {
                            stepObj.stepDetails = payload.stepDetails;
                            ctrl.customerId = payload.stepDetails.CustomerId;
                            ctrl.saleZoneId = payload.stepDetails.SaleZoneId;
                            ctrl.effectiveOn = payload.stepDetails.EffectiveOn;
                            ctrl.customerZoneRate = payload.stepDetails.CustomerZoneRate;
                        }
                        checkValidation();
                    }

                };

                api.applyChanges = function (changes) {
                    ctrl.customerId = changes.CustomerId;
                    ctrl.saleZoneId = changes.SaleZoneId;
                    ctrl.effectiveOn = changes.EffectiveOn;
                    ctrl.customerZoneRate = changes.CustomerZoneRate;
                    stepObj.stepDetails = changes;
                };

                api.checkValidation = function () {
                    return checkValidation();
                };

                api.getData = function () {
                    return stepObj.stepDetails;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function checkValidation() {
                if (ctrl.customerId == undefined) {
                    return "Missing customerId mapping.";
                }
                if (ctrl.saleZoneId == undefined) {
                    return "Missing saleZoneId mapping.";
                }
                if (ctrl.effectiveOn == undefined) {
                    return "Missing effectiveOn mapping.";
                }
                if (ctrl.customerZoneRate == undefined) {
                    return "Missing customerZoneRate mapping.";
                }
                return null;
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);