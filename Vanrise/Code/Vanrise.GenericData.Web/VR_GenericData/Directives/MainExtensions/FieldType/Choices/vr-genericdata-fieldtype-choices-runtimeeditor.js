'use strict';
app.directive('vrGenericdataFieldtypeChoicesRuntimeeditor', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {

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

            return '<vr-columns colnum="{{ctrl.normalColNum}}" ng-if="!scopeModel.showAsLabel">' +
                '<vr-select datatextfield="Text" label="{{ctrl.label}}" datavaluefield="Value" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" datasource="ctrl.datasource" onselectionchanged="ctrl.onChoiceSelectionChanged"'
                + multipleselection + ' isrequired="ctrl.isrequired"></vr-select>' +
                '</vr-columns>'

                + '<vr-columns colnum="{{ctrl.normalColNum}}" haschildcolumns ng-if="scopeModel.showAsLabel">'
                + '<vr-columns colnum="3"> '
                + '<vr-label> {{ctrl.label}}</vr-label>'
                + '</vr-columns>'
                + '<vr-columns colnum="1">'
                + '<vr-label> : </vr-label>'
                + '</vr-columns>'
                + '<vr-columns colnum="8">'
                + '<vr-label isvalue> {{scopeModel.valueAsString}} </vr-label>'
                + '</vr-columns>'
                + '</vr-columns>';
        }

        function choicesCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var fieldName;
            var genericContext;
            var fieldType;
            var fieldValue;

            var selectorAPI;


            function initializeController() {
                $scope.scopeModel = {};

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                if ($attrs.selectionmode == "single") {
                    defineScopeForSingleMode();
                }
            }

            function defineScopeForSingleMode() {
                ctrl.onChoiceSelectionChanged = function (selectedChoice) {

                    if (genericContext != undefined && genericContext.notifyFieldValueChanged != undefined && typeof (genericContext.notifyFieldValueChanged) == "function") {
                        var valueAsArray;
                        if (selectedChoice != undefined) {
                            valueAsArray = [selectedChoice['Value']];
                        }

                        var changedField = { fieldName: fieldName, fieldValues: valueAsArray };
                        genericContext.notifyFieldValueChanged(changedField);
                    }
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();
                    var promises = [];

                    var filter = {};
                   
                    if (payload != undefined) {
                        ctrl.label = payload.fieldTitle;
                        filter = payload.filter;
                        fieldType = payload.fieldType;
                        fieldValue = payload.fieldValue;
                        fieldName = payload.fieldName;
                        genericContext = payload.genericContext;
                        $scope.scopeModel.showAsLabel = payload.showAsLabel;
                        if (payload.showAsLabel) {
                            promises.push(getFieldTypeDescription());
                        }
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
                    return UtilsService.waitMultiplePromises(promises);
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

            function getFieldTypeDescription() {
                return VR_GenericData_DataRecordFieldAPIService.GetFieldTypeDescription(fieldType, fieldValue).then(function (response) {
                    $scope.scopeModel.valueAsString = response;
                });
            }
        }

        return directiveDefinitionObject;
    }]);