'use strict';
app.directive('vrWhsRoutingDatatransformationMatchingsuppliercodematchPreview', ['UtilsService', 'VRUIUtilsService',
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
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_Routing/Directives/TransformationSteps/MatchingSupplierCodeMatchStep/Templates/MatchingSupplierCodeMatchStepPreviewTemplate.html';
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
                            ctrl.supplierId = payload.stepDetails.SupplierId;
                            ctrl.supplierANumberId = payload.stepDetails.SupplierANumberId;
                            ctrl.effectiveOn = payload.stepDetails.EffectiveOn;
                            ctrl.matchingSupplierId = payload.stepDetails.MatchingSupplierId;
                            ctrl.supplierCode = payload.stepDetails.SupplierCode;
                            ctrl.supplierZoneId = payload.stepDetails.SupplierZoneId;
                        }
                        checkValidation();
                    }

                };

                api.applyChanges = function (changes) {
                    ctrl.number = changes.Number;
                    ctrl.supplierId = changes.SupplierId;
                    ctrl.supplierANumberId = changes.SupplierANumberId;
                    ctrl.effectiveOn = changes.EffectiveOn;
                    ctrl.matchingSupplierId = changes.MatchingSupplierId;
                    ctrl.supplierCode = changes.SupplierCode;
                    ctrl.supplierZoneId = changes.SupplierZoneId;
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
                if (ctrl.supplierId == undefined) {
                    return "Missing supplierId mapping.";
                }

                if (ctrl.supplierANumberId == undefined) {
                    return "Missing supplierANumberId mapping.";
                }

                if (ctrl.effectiveOn == undefined) {
                    return "Missing effectiveOn mapping.";
                }
                if (ctrl.supplierCode == undefined) {
                    return "Missing supplierCode mapping.";
                }
                if (ctrl.supplierZoneId == undefined) {
                    return "Missing supplierZoneId mapping.";
                }
                return null;
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);