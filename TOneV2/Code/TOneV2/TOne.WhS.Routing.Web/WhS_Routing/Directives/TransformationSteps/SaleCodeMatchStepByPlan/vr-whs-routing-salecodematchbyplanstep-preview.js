'use strict';

app.directive('vrWhsRoutingSalecodematchbyplanstepPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SaleCodeMatchByPlanStepPreviewCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_Routing/Directives/TransformationSteps/SaleCodeMatchStepByPlan/Templates/SaleCodeMatchByPlanStepPreviewTemplate.html';
            }
        };

        function SaleCodeMatchByPlanStepPreviewCtor(ctrl, $scope) {
            this.initializeController = initializeController;

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

                array.push({
                    FieldName: "EffectiveOn",
                    Value: data.CDPNOut
                });
                array.push({
                    FieldName: "SellingNumberPlan",
                    Value: data.CDPNIn
                });
                array.push({
                    FieldName: "CDPN",
                    Value: data.SwitchId
                });
                array.push({
                    FieldName: "CGPN",
                    Value: data.InputCDPN
                });
            }
            function fillOutputFieldsArray(data, array) {

                array.push({
                    FieldName: data.CustomerCDPN,
                    Value: "SaleCode"
                });
                array.push({
                    FieldName: data.SupplierCDPN,
                    Value: "SaleZoneId"
                });
                array.push({
                    FieldName: data.CustomerCDPN,
                    Value: "OriginatingSaleCode"
                });
                array.push({
                    FieldName: data.SupplierCDPN,
                    Value: "OriginatingSaleZoneId"
                });
            }
        }

        return directiveDefinitionObject;
    }
]);