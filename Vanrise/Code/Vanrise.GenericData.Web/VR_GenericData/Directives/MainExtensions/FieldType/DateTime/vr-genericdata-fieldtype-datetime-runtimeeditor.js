'use strict';
app.directive('vrGenericdataFieldtypeDatetimeRuntimeeditor', ['UtilsService', 'VR_GenericData_DateTimeDataTypeEnum', function (UtilsService, VR_GenericData_DateTimeDataTypeEnum) {

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
            $scope.scopeModel = {};
            var ctor = new dateTimeCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'runtimeEditorCtrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        template: function (element, attrs) {
            return getDirectiveTemplate(attrs);
        }
    };

    function dateTimeCtor(ctrl, $scope, $attrs) {

        function initializeController() {

            $scope.scopeModel.value;

            if (ctrl.selectionmode != 'single') {
                defineScopeForMultiModes();
            }

            if (ctrl.onReady != undefined) {
                ctrl.onReady(getDirectiveAPI());
            }
        }

        function defineScopeForMultiModes() {
            $scope.scopeModel.values = [];
            $scope.scopeModel.isAddButtonDisabled = true;

            $scope.scopeModel.addValue = function () {
                $scope.scopeModel.values.push(getDataItem($scope.scopeModel.value));
                $scope.scopeModel.value = undefined;
            };

            $scope.scopeModel.validateValue = function () {

                if ($scope.scopeModel.value == undefined || $scope.scopeModel.value == null || $scope.scopeModel.value == '') {
                    $scope.scopeModel.isAddButtonDisabled = true;
                    return null;
                }
                for (var i = 0; i < $scope.scopeModel.values.length; i++) {
                    if (UtilsService.areDateTimesEqual($scope.scopeModel.values[i].value, $scope.scopeModel.value)) {
                        $scope.scopeModel.isAddButtonDisabled = true;
                        return 'Value already exists';
                    }
                }
                $scope.scopeModel.isAddButtonDisabled = false;
                return null;
            };
        }

        function getDirectiveAPI() {
            var api = {};

            api.load = function (payload) {
                var fieldType;
                var fieldValue;

                if (payload != undefined) {
                    $scope.scopeModel.label = payload.fieldTitle;
                    fieldType = payload.fieldType;
                    fieldValue = payload.fieldValue;
                    var dataTypes = UtilsService.getArrayEnum(VR_GenericData_DateTimeDataTypeEnum);
                    $scope.scopeModel.fieldType = UtilsService.getItemByVal(dataTypes, fieldType.DataType, 'value');
                }

                if (fieldValue != undefined) {
                    if (ctrl.selectionmode == "dynamic") {
                        angular.forEach(fieldValue.Values, function (val) {
                            $scope.scopeModel.values.push(getDataItem(val));
                        });
                    }
                    else if (ctrl.selectionmode == "multiple") {
                        for (var i = 0; i < fieldValue.length; i++) {
                            $scope.scopeModel.values.push(getDataItem(fieldValue[i]));
                        }
                    }
                    else {
                        if ($scope.scopeModel.fieldType.type != "time")
                            $scope.scopeModel.value = fieldValue;
                        else {
                            $scope.scopeModel.value = convertTimeToDate(fieldValue);
                        }
                    }
                }

                function convertTimeToDate(time) {
                    var date = new Date();
                    date.setHours(time.Hour);
                    date.setMinutes(time.Minute);
                    date.setSeconds(time.Second);
                    date.setMilliseconds(time.MilliSecond);
                    return date;
                }
            };

            api.getData = function () {
                var retVal;

                if (ctrl.selectionmode == "dynamic") {
                    if ($scope.scopeModel.values.length > 0) {
                        retVal = {
                            $type: "Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions",
                            Values: UtilsService.getPropValuesFromArray($scope.scopeModel.values, 'value')
                        };
                    }
                }
                else if (ctrl.selectionmode == "multiple") {
                    if ($scope.scopeModel.values.length > 0) {
                        retVal = UtilsService.getPropValuesFromArray($scope.scopeModel.values, 'value')
                    }
                }
                else {
                    retVal = $scope.scopeModel.value;
                }

                return retVal;
            };

            return api;
        }

        function getDataItem(value) {
            return {
                id: $scope.scopeModel.values.length + 1,
                value: value,
                displayValue: getDateTimeString(value)
            };

            function getDateTimeString(dateTime) {
                var type = $scope.scopeModel.fieldType.type;
                var dateTimeObject = new Date(dateTime);

                switch (type) {
                    case 'dateTime':
                        return dateTimeObject.toString();
                    case 'date':
                        return dateTimeObject.toDateString();
                    case 'time':
                        return dateTime.Hour + ':' + dateTime.Minute;
                }
            }
        }

        this.initializeController = initializeController;
    }

    function getDirectiveTemplate(attrs) {
        if (attrs.selectionmode == 'single') {
            return getSingleSelectionModeTemplate();
        }
        else {
            return '<vr-columns colnum="{{runtimeEditorCtrl.normalColNum * 4}}">'
                    + '<vr-row>'
                        + getSingleSelectionModeTemplate()
                        + '<vr-columns withemptyline>'
                            + '<vr-button type="Add" data-onclick="scopeModel.addValue" standalone vr-disabled="scopeModel.isAddButtonDisabled"></vr-button>'
                        + '</vr-columns>'
                    + '</vr-row>'
                    + '<vr-row>'
                        + '<vr-columns colnum="{{runtimeEditorCtrl.normalColNum * 2}}">'
                            + '<vr-datalist maxitemsperrow="6" datasource="scopeModel.values" autoremoveitem="true">{{dataItem.displayValue}}</vr-datalist>'
                        + '</vr-columns>'
                    + '</vr-row>'
                + '</vr-columns>';
        }

        function getSingleSelectionModeTemplate() {
            return '<vr-columns colnum="{{runtimeEditorCtrl.normalColNum}}" ng-if="scopeModel.fieldType != undefined && scopeModel.label != undefined ">'
                        + '<vr-label>{{scopeModel.label}}</vr-label>'
                        + '<vr-directivewrapper directive="\'vr-datetimepicker\'" type="{{scopeModel.fieldType.type}}" value="scopeModel.value" customvalidate="scopeModel.validateValue()" isrequired="runtimeEditorCtrl.isrequired"></vr-directivewrapper>'
                + '</vr-columns>';
        }
    }

    return directiveDefinitionObject;
}]);