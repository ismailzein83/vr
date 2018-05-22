'use strict';
app.directive('vrGenericdataFieldtypeChoicesRuntimeeditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectionmode: '@',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                ctrl.selectedvalues;
                if (ctrl.selectionmode == "dynamic" || ctrl.selectionmode == "multiple")
                    ctrl.selectedvalues = [];

                ctrl.datasource = [];
                var ctor = new choicesCtor(ctrl, $scope, $attrs);
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
            template: function (element, attrs) {
                return getTemplate(attrs);
            }

        };

        function getTemplate(attrs) {
            var multipleselection = "";
            //var label = "";
            if (attrs.selectionmode == "dynamic" || attrs.selectionmode == "multiple") {
                //label = "";
                multipleselection = "ismultipleselection";
            }
            //var required = "";
            //if (attrs.isrequired != undefined)
            //    required = "isrequired";

            //var hideremoveicon = "";
            //if (attrs.hideremoveicon != undefined)
            //    hideremoveicon = "hideremoveicon";

            //var isRequired = (attrs.selectionmode == "single" && attrs.isrequired != undefined) ? 'isrequired="ctrl.isrequired"' : '';

            return '<vr-columns colnum="{{ctrl.normalColNum}}">' +
            '<vr-select datatextfield="Text" label="{{ctrl.label}}" datavaluefield="Value" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" datasource="ctrl.datasource" '
                + multipleselection + ' isrequired="ctrl.isrequired"></vr-select>' +
                '</vr-columns>';
        }

        function choicesCtor(ctrl, $scope, $attrs) {

            var selectorApi;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorApi = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorApi.clearDataSource();

                    var filter = {};
                    var fieldType;
                    var fieldValue;

                    if (payload != undefined) {
                        ctrl.label = payload.fieldTitle;
                        filter = payload.filter;
                        fieldType = payload.fieldType;
                        fieldValue = payload.fieldValue;
                    }

                    if (fieldType != undefined) {
                        for (var i = 0; i < fieldType.Choices.length; i++)
                            ctrl.datasource.push(fieldType.Choices[i]);

                        if (fieldValue != undefined) {
                            if (ctrl.selectionmode == "dynamic") {
                                if (fieldValue.Values != undefined) {
                                    for (var i = 0; i < fieldValue.Values.length; i++) {
                                        var selectedValue = UtilsService.getItemByVal(ctrl.datasource, fieldValue.Values[i], "Value");
                                        if (selectedValue != null)
                                            ctrl.selectedvalues.push(selectedValue);
                                    }
                                }
                            }
                            else if (ctrl.selectionmode == "multiple") {
                                for (var i = 0; i < fieldValue.length; i++) {
                                    var selectedValue = UtilsService.getItemByVal(ctrl.datasource, fieldValue[i], "Value");
                                    if (selectedValue != null)
                                        ctrl.selectedvalues.push(selectedValue);
                                }
                            }
                            else if (ctrl.selectionmode == "single") {
                                var selectedValue = UtilsService.getItemByVal(ctrl.datasource, fieldValue, "Value");
                                if (selectedValue != null)
                                    ctrl.selectedvalues = selectedValue;
                            }
                        }
                    }
                };

                api.getData = function () {
                    var retVal;

                    if (ctrl.selectionmode == "dynamic") {
                        if (ctrl.selectedvalues.length > 0) {
                            retVal = {
                                $type: "Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions",
                                Values: UtilsService.getPropValuesFromArray(ctrl.selectedvalues, 'Value')
                            };
                        }
                    }
                    else if (ctrl.selectionmode == "multiple") {
                        if (ctrl.selectedvalues.length > 0) {
                            retVal = UtilsService.getPropValuesFromArray(ctrl.selectedvalues, 'Value');
                        }
                    }
                    else if (ctrl.selectionmode == "single") {
                        if (ctrl.selectedvalues != undefined) {
                            retVal = ctrl.selectedvalues['Value'];
                        }
                    }

                    return retVal;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);