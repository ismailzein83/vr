'use strict';

app.directive('vrWhsBeGetcdpnsforidentificationstepPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GetCDPNsForIdentificationStepPreviewCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/MappingSteps/GetCDPNsForIdentification/Templates/GetCDPNsForIdentificationStepPreviewTemplate.html';
            }
        };

        function GetCDPNsForIdentificationStepPreviewCtor(ctrl, $scope) {
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
                    FieldName: "SwitchId",
                    Value: data.SwitchId
                });
                array.push({
                    FieldName: "CDPN",
                    Value: data.InputCDPN
                });
                array.push({
                    FieldName: "CDPN In",
                    Value: data.CDPNIn
                });
                array.push({
                    FieldName: "CDPN Out",
                    Value: data.CDPNOut
                });
            }
            function fillOutputFieldsArray(data, array) {

                array.push({
                    FieldName: data.CustomerCDPN,
                    Value: "Customer CDPN"
                });
                array.push({
                    FieldName: data.SupplierCDPN,
                    Value: "Supplier CDPN"
                });
                array.push({
                    FieldName: data.OutputCDPN,
                    Value: "CDPN"
                });
            }

        }

        return directiveDefinitionObject;
    }
]);