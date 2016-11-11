'use strict';
app.directive('vrGenericdataFieldtypeDatetime', ['UtilsService','VR_GenericData_DateTimeDataTypeEnum',
    function (UtilsService, VR_GenericData_DateTimeDataTypeEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new datetimeTypeCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DateTime/Templates/DateTimeFieldTypeTemplate.html';
            }

        };

        function datetimeTypeCtor(ctrl, $scope) {

            function initializeController() {

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var dataTypeValue;
                    if (payload != undefined) {
                        dataTypeValue = payload.DataType;
                        ctrl.isNullable = payload.IsNullable;
                    }

                    ctrl.dateTimeDataTypes = UtilsService.getArrayEnum(VR_GenericData_DateTimeDataTypeEnum);
                    if (dataTypeValue != undefined) {
                        ctrl.selectedDateTimeDataType = UtilsService.getItemByVal(ctrl.dateTimeDataTypes, dataTypeValue, 'value');
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType,Vanrise.GenericData.MainExtensions",
                        DataType: ctrl.selectedDateTimeDataType.value,
                        IsNullable: ctrl.isNullable
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);