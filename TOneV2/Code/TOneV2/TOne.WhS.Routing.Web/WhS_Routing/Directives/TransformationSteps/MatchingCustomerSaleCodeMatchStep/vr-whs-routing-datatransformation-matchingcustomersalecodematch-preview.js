'use strict';
app.directive('vrWhsRoutingDatatransformationMatchingcustomersalecodematchPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new SaleCodeMatchStepCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_Routing/Directives/TransformationSteps/MatchingCustomerSaleCodeMatchStep/Templates/MatchingCustomerSaleCodeMatchStepPreviewTemplate.html';
            }

        };

        function SaleCodeMatchStepCtor(ctrl, $scope) {
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
                            ctrl.number = payload.stepDetails.Number;
                            ctrl.customerId = payload.stepDetails.CustomerId;
                            ctrl.customerANumberId = payload.stepDetails.CustomerANumberId;
                            ctrl.effectiveOn = payload.stepDetails.EffectiveOn;
                            ctrl.matchingCustomerId = payload.stepDetails.MatchingCustomerId;
                            ctrl.customerSellingNumberPlanId = payload.stepDetails.CustomerSellingNumberPlanId;
                            ctrl.saleCode = payload.stepDetails.SaleCode;
                            ctrl.saleZoneId = payload.stepDetails.SaleZoneId;
                            ctrl.masterSaleCode = payload.stepDetails.MasterSaleCode;
                            ctrl.masterSaleZoneId = payload.stepDetails.MasterSaleZoneId;
                        }
                        checkValidation();
                    }

                };

                api.applyChanges = function (changes) {
                    ctrl.number = changes.Number;
                    ctrl.customerId = changes.CustomerId;
                    ctrl.customerANumberId = changes.CustomerANumberId;
                    ctrl.effectiveOn = changes.EffectiveOn;
                    ctrl.matchingCustomerId = changes.MatchingCustomerId;
                    ctrl.customerSellingNumberPlanId = changes.CustomerSellingNumberPlanId;
                    ctrl.saleCode = changes.SaleCode;
                    ctrl.saleZoneId = changes.SaleZoneId;
                    ctrl.masterSaleCode = changes.MasterSaleCode;
                    ctrl.masterSaleZoneId = changes.MasterSaleZoneId;
                    stepObj.stepDetails = changes;
                };

                api.checkValidation = function () {
                    return checkValidation();
                };

                api.getData = function () {
                    return stepObj.stepDetails
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function checkValidation() {
                if (ctrl.number == undefined) {
                    return "Missing number mapping.";
                }

                if (ctrl.customerId == undefined) {
                    return "Missing customerId mapping.";
                }

                if (ctrl.customerANumberId == undefined) {
                    return "Missing customerANumberId mapping.";
                }

                if (ctrl.effectiveOn == undefined) {
                    return "Missing effectiveOn mapping.";
                }

                if (ctrl.matchingCustomerId == undefined) {
                    return "Missing matchingCustomerId mapping.";
                }

                if (ctrl.customerSellingNumberPlanId == undefined) {
                    return "Missing customerSellingNumberPlanId mapping.";
                }

                if (ctrl.saleCode == undefined) {
                    return "Missing saleCode mapping.";
                }

                if (ctrl.saleZoneId == undefined) {
                    return "Missing saleZoneId mapping.";
                }

                if (ctrl.masterSaleCode == undefined) {
                    return "Missing masterSaleCode mapping.";
                }

                if (ctrl.masterSaleZoneId == undefined) {
                    return "Missing masterSaleZoneId mapping.";
                }

                return null;
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);