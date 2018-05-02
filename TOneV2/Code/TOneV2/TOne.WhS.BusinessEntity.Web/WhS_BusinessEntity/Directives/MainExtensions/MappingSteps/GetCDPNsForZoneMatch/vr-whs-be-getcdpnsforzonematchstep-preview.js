'use strict';

app.directive('vrWhsBeGetcdpnsforzonematchstepPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new GetCDPNsForZoneMatchStepPreviewCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/MappingSteps/GetCDPNsForZoneMatch/Templates/GetCDPNsForZoneMatchStepPreviewTemplate.html';
            }
        };

        function GetCDPNsForZoneMatchStepPreviewCtor(ctrl, $scope) {
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
                    FieldName: "Effective Time",
                    Value: data.EffectiveTime
                });
                array.push({
                    FieldName: "CDPN",
                    Value: data.CDPN
                });
                array.push({
                    FieldName: "CDPN In",
                    Value: data.CDPNIn
                });
                array.push({
                    FieldName: "CDPN Out",
                    Value: data.CDPNOut
                });
                array.push({
                    FieldName: "SwitchId",
                    Value: data.SwitchId
                });
                array.push({
                    FieldName: "CustomerId",
                    Value: data.CustomerId
                });
                array.push({
                    FieldName: "SupplierId",
                    Value: data.SupplierId
                });
            }
            function fillOutputFieldsArray(data, array) {

                array.push({
                    FieldName: data.SaleZoneCDPN,
                    Value: "Sale Zone CDPN"
                });
                array.push({
                    FieldName: data.SupplierZoneCDPN,
                    Value: "Supplier Zone CDPN"
                });
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }
]);