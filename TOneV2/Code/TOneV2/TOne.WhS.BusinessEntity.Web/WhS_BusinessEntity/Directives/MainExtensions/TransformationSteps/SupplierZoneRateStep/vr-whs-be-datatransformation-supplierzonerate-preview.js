'use strict';
app.directive('vrWhsBeDatatransformationSupplierzoneratePreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new SupplierZoneRatePreviewStepCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/TransformationSteps/SupplierZoneRateStep/Templates/SupplierZoneRateStepPreviewTemplate.html';
            }

        };

        function SupplierZoneRatePreviewStepCtor(ctrl, $scope) {
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
                            ctrl.supplierId = payload.stepDetails.SupplierId;
                            ctrl.supplierZoneId = payload.stepDetails.SupplierZoneId;
                            ctrl.effectiveOn = payload.stepDetails.EffectiveOn;
                            ctrl.supplierZoneRate = payload.stepDetails.SupplierZoneRate;
                        }
                        checkValidation();
                    }

                };

                api.applyChanges = function (changes) {
                    ctrl.supplierId = changes.SupplierId;
                    ctrl.supplierZoneId = changes.SupplierZoneId;
                    ctrl.effectiveOn = changes.EffectiveOn;
                    ctrl.supplierZoneRate = changes.SupplierZoneRate;
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
                if (ctrl.supplierId == undefined) {
                    return "Missing supplierId mapping.";
                }
                if (ctrl.supplierZoneId == undefined) {
                    return "Missing supplierZoneId mapping.";
                }
                if (ctrl.effectiveOn == undefined) {
                    return "Missing effectiveOn mapping.";
                }
                if (ctrl.supplierZoneRate == undefined) {
                    return "Missing supplierZoneRate mapping.";
                }
                return null;
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);