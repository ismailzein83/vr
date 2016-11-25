'use strict';

app.directive('vrNpDatatransformationMasterplansalecodematchPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new MasterPlanSaleCodeMatchStepPreviewCtor(ctrl, $scope);
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
                return "/Client/Modules/VR_NumberingPlan/Directives/TransformationSteps/MasterPlanSaleCodeMatchStep/Templates/MasterPlanSaleCodeMatchStepPreviewTemplate.html";
            }

        };

        function MasterPlanSaleCodeMatchStepPreviewCtor(ctrl, $scope) {
            var stepObj = {};

            function initializeController() {
                ctrl.inputFields = [];
                ctrl.outputFields = [];
                 
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.inputFields = [];
                    ctrl.outputFields = [];

                    if (payload != undefined) {
                        if (payload.stepDetails != undefined) {
                            stepObj.stepDetails = payload.stepDetails;

                            fillInputFieldsArray(payload.stepDetails, ctrl.inputFields);
                            fillOutputFieldsArray(payload.stepDetails, ctrl.outputFields);
                        }
                        checkValidation();
                    }

                };

                api.applyChanges = function (changes) {
                    ctrl.inputFields = [];
                    ctrl.outputFields = [];

                    fillInputFieldsArray(changes, ctrl.inputFields);
                    fillOutputFieldsArray(changes, ctrl.outputFields);

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

                var misisngFieldsArray = [];

                for (var index = 0 ; index < ctrl.inputFields.length; index++) {
                    if (ctrl.inputFields[index].Value == undefined) {
                        misisngFieldsArray.push(ctrl.inputFields[index].FieldName);
                    }
                }

                for (var index = 0 ; index < ctrl.outputFields.length; index++) {
                    if (ctrl.outputFields[index].FieldName == undefined) {
                        misisngFieldsArray.push(ctrl.outputFields[index].Value);
                    }
                }

                if (misisngFieldsArray.length == 1)
                    return "Field " + misisngFieldsArray[0] + " is required!!";

                if (misisngFieldsArray.length > 1)
                    return "Fields " + misisngFieldsArray.join(", ") + " are required!!";

                return null;
            }
            function fillInputFieldsArray(data, array) {

                if (data.Number)
                {
                    array.push({
                        FieldName: "Number",
                        Value: data.Number
                    });
                }
                if (data.EffectiveOn)
                {
                    array.push({
                        FieldName: "Effective On",
                        Value: data.EffectiveOn
                    });
                }
            }
            function fillOutputFieldsArray(data, array) {

                if (data.SaleCode)
                {
                    array.push({
                        FieldName: data.SaleCode,
                        Value: "Sale Code"
                    });
                }
                if (data.SaleZoneId)
                {
                    array.push({
                        FieldName: data.SaleZoneId,
                        Value: "Sale Zone Id"
                    });
                }
                if (data.SellingNumberPlanId)
                {
                    array.push({
                        FieldName: data.SellingNumberPlanId,
                        Value: "Selling Number Plan Id"
                    });
                }
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }
]);