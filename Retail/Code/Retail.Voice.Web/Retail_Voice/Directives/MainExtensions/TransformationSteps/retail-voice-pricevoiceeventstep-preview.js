'use strict';
app.directive('retailVoicePricevoiceeventstepPreview', ['UtilsService', 'VRUIUtilsService',
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
                return '/Client/Modules/Retail_Voice/Directives/MainExtensions/TransformationSteps/Templates/PriceVoiceEventStepPreviewTemplate.html';
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
                    FieldName: "AccountId",
                    Value: data.AccountId
                });
                array.push({
                    FieldName: "ServiceTypeId",
                    Value: data.ServiceTypeId
                });
                array.push({
                    FieldName: "RawCDR",
                    Value: data.RawCDR
                });
                array.push({
                    FieldName: "MappedCDR",
                    Value: data.MappedCDR
                });
                array.push({
                    FieldName: "Duration",
                    Value: data.Duration
                });
                array.push({
                    FieldName: "EventTime",
                    Value: data.EventTime
                });
            }
            function fillOutputFieldsArray(data, array) {
                
                if (data.PackageId != undefined) {
                    array.push({
                        FieldName: data.PackageId,
                        Value: "Package Id"
                    });
                }
                if (data.UsageChargingPolicyId != undefined) {
                    array.push({
                        FieldName: data.UsageChargingPolicyId,
                        Value: "Charging Policy Id"
                    });
                }
                if (data.Rate != undefined) {
                    array.push({
                        FieldName: data.Rate,
                        Value: "Rate"
                    });
                }
                if (data.Amount != undefined) {
                    array.push({
                        FieldName: data.Amount,
                        Value: "Amount"
                    });
                }
                if (data.RateTypeId != undefined) {
                    array.push({
                        FieldName: data.RateTypeId,
                        Value: "Rate Type Id"
                    });
                }
                if (data.CurrencyId != undefined) {
                    array.push({
                        FieldName: data.CurrencyId,
                        Value: "Currency Id"
                    });
                }
                if (data.VoiceEventPricedParts != undefined) {
                    array.push({
                        FieldName: data.VoiceEventPricedParts,
                        Value: "Voice Event Priced Parts"
                    });
                }
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }
]);