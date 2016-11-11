'use strict';
app.directive('vrGenericdataFieldtypeNumber', ['VR_GenericData_FieldNumberDataTypeEnum', 'UtilsService', 'VR_GenericData_FieldNumberDataPrecisionEnum',
    function (VR_GenericData_FieldNumberDataTypeEnum, UtilsService, VR_GenericData_FieldNumberDataPrecisionEnum) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            var ctor = new numberTypeCtor(ctrl, $scope);
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
            return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Number/Templates/NumberFieldTypeTemplate.html';
        }
    };

    function numberTypeCtor(ctrl, $scope) {

        var selectorAPI;
        var precisionSelectorAPI;

        function initializeController() {
            ctrl.numberDataTypes = [];

            ctrl.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };

            ctrl.onPrecisionSelectorReady = function (api) {
                precisionSelectorAPI = api;
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var dataTypeValue;
                var dataPrecisionValue;

                if (payload != undefined) {
                    dataTypeValue = payload.DataType;
                    ctrl.isNullable = payload.IsNullable;
                    dataPrecisionValue = payload.DataPrecision;
                }

                selectorAPI.clearDataSource();
                ctrl.numberDataTypes = UtilsService.getArrayEnum(VR_GenericData_FieldNumberDataTypeEnum);
                ctrl.numberDataPrecision = UtilsService.getArrayEnum(VR_GenericData_FieldNumberDataPrecisionEnum);

                if (dataTypeValue != undefined) {
                    ctrl.selectedNumberDataType = UtilsService.getItemByVal(ctrl.numberDataTypes, dataTypeValue, 'value');
                }

                if (dataPrecisionValue != undefined) {
                    ctrl.selectedNumberDataPrecision = UtilsService.getItemByVal(ctrl.numberDataPrecision, dataPrecisionValue, 'value');
                }
            };

            api.getData = function () {
                return {
                    $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions",
                    DataType: ctrl.selectedNumberDataType.value,
                    IsNullable: ctrl.isNullable,
                    DataPrecision: ctrl.selectedNumberDataPrecision != undefined ? ctrl.selectedNumberDataPrecision.value : undefined
                };
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                ctrl.onReady(api);
            }
        }

        this.initializeController = initializeController;
    }
 }]);