'use strict';
app.directive('retailVoiceInternationalidentificationstepPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new PriceVoiceEventStepPreviewCtor(ctrl, $scope);
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
                return '/Client/Modules/Retail_Voice/Directives/MainExtensions/TransformationSteps/InternationalIdentification/Templates/InternationalIdentificationStepPreviewTemplate.html';
            }
        };

        function PriceVoiceEventStepPreviewCtor(ctrl, $scope) {
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
                    var currentInputItem = ctrl.inputFields[index];
                    if (currentInputItem.Value == undefined) {
                        misisngFieldsArray.push(currentInputItem.FieldName);
                    }
                }

                for (var index = 0 ; index < ctrl.outputFields.length; index++) {
                    var currentOutputItem = ctrl.outputFields[index];
                    if (currentOutputItem.FieldName == undefined) {
                        misisngFieldsArray.push(currentOutputItem.Value);
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
                    FieldName: "RawCDR",
                    Value: data.RawCDR
                });
                array.push({
                    FieldName: "OtherPartyNumber",
                    Value: data.OtherPartyNumber
                });
            }
            function fillOutputFieldsArray(data, array) {

                if (data.IsInternational != undefined) {
                    array.push({
                        FieldName: data.IsInternational,
                        Value: "Is International"
                    });
                }
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }
]);